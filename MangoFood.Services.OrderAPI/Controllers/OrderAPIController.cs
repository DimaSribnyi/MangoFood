using AutoMapper;
using MangoFood.MessageBus;
using MangoFood.Services.OrderAPI.Data;
using MangoFood.Services.OrderAPI.Models;
using MangoFood.Services.OrderAPI.Models.DTO;
using MangoFood.Services.OrderAPI.Services.IService;
using MangoFood.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace MangoFood.Services.OrderAPI.Controllers
{
    [Route("api/Order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IProductService _productService;
        private readonly IMessageBusService _messageBusService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private ResponseDTO _response;

        public OrderAPIController(AppDbContext appDbContext, IMapper mapper, 
            IProductService productService, IMessageBusService messageBusService, IConfiguration configuration)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _response = new ResponseDTO();
            _productService = productService;
            _messageBusService = messageBusService;
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet("GetOrders")]
        public async Task<ActionResult<ResponseDTO>> GetOrders(string userId = "")
        {
            try
            {
                IEnumerable<OrderHeader> orders;
                if (User.IsInRole(SD.RoleAdmin))
                {
                    orders = await _appDbContext.OrderHeaders.Include(u => u.OrderDetails)
                        .AsNoTracking().OrderByDescending(o => o.OrderHeaderId).ToListAsync();
                }
                else
                {
                    orders = await _appDbContext.OrderHeaders.Include(u => u.OrderDetails)
                        .AsNoTracking().Where(o => o.UserId == userId)
                        .OrderByDescending(o => o.OrderHeaderId).ToListAsync();
                }

                _response.Result = _mapper.Map<IEnumerable<OrderHeaderDTO>>(orders);
            }
            catch (Exception ex)
            {
                CatchEx(ex);
            }

            return _response;
        }

        [Authorize]
        [HttpGet("GetOrder/{id:int}")]
        public async Task<ActionResult<ResponseDTO>> GetOrderById(int id)
        {
            try
            {
                var order = await _appDbContext.OrderHeaders.Include(u => u.OrderDetails).AsNoTracking()
                    .FirstOrDefaultAsync(o => o.OrderHeaderId == id);

                if (order == null)
                {
                    _response.Success = false;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<OrderHeaderDTO>(order);
            }
            catch (Exception ex)
            {
                CatchEx(ex);
            }

            return _response;
        }

        [Authorize]
        [HttpPut("UpdateOrderStatus/{orderId:int}")]
        public async Task<ActionResult<ResponseDTO>> UpdateOrderStatus(int orderId, [FromBody] string status)
        {
            try
            {
                var orderHeader = await _appDbContext.OrderHeaders.FirstAsync(o => o.OrderHeaderId ==  orderId);    
                if(orderHeader == null)
                {
                    _response.Success = false;
                    return NotFound(_response);
                }

                orderHeader.Status = status;
                await _appDbContext.SaveChangesAsync(); 
            }
            catch (Exception ex)
            {
                CatchEx(ex);
            }

            return _response;
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ActionResult<ResponseDTO>> CreateOrder(ShoppingCartDTO cartDTO)
        {
            try
            {
                var orderHeaderDTO = _mapper.Map<OrderHeaderDTO>(cartDTO.CartHeader);
                orderHeaderDTO.OrderTime = DateTime.Now;
                orderHeaderDTO.Status = SD.Status_Pending;
                orderHeaderDTO.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDTO>>(cartDTO.CartDetails);

                var order = _mapper.Map<OrderHeader>(orderHeaderDTO);
                _appDbContext.Add(order);
                await _appDbContext.SaveChangesAsync();

                orderHeaderDTO.OrderHeaderId = order.OrderHeaderId;
                _response.Result = orderHeaderDTO;
            }
            catch (Exception ex)
            {
                CatchEx(ex);
            }

            return _response;
        }

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ActionResult<ResponseDTO>> CreateStripeSession(StripeRequestDTO stripeRequest)
        {
            try
            {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequest.ApprovedUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    CancelUrl = stripeRequest.CancelUrl,
                    Mode = "payment"
                };

                var discountObj = new List<SessionDiscountOptions> 
                {
                    new SessionDiscountOptions
                    {
                        Coupon = stripeRequest.OrderHeader.CouponCode
                    }
                };

                foreach(var item in stripeRequest.OrderHeader.OrderDetails)
                {
                    var sessionListItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(100 * item.ProductPrice), // 20.99 -> 2099
                            Currency = "usd", 
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.ProductName
                            }
                        }, 
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionListItem);
                }

                if(stripeRequest.OrderHeader.Discount > 0)
                {
                    options.Discounts = discountObj;
                }

                var service = new SessionService();
                var session = await service.CreateAsync(options);
                stripeRequest.SessionUrl = session.Url;

                var orderHeader = await _appDbContext.OrderHeaders
                    .FirstAsync(h => h.OrderHeaderId == stripeRequest.OrderHeader.OrderHeaderId);

                orderHeader.StripeSessionId = session.Id;
                await _appDbContext.SaveChangesAsync();

                _response.Result = stripeRequest;
            }
            catch (Exception ex)
            {
                CatchEx(ex);
            }
            return _response;
        }

        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ActionResult<ResponseDTO>> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                var orderHeader = await _appDbContext.OrderHeaders
                    .FirstAsync(h => h.OrderHeaderId == orderHeaderId);

                var service = new SessionService();
                var session = service.Get(orderHeader.StripeSessionId);

                var paymentInentService = new PaymentIntentService();
                var paymentIntent = await paymentInentService.GetAsync(session.PaymentIntentId);

                if(paymentIntent.Status == "succeeded")
                {
                    orderHeader.PaymentInentId = paymentIntent.Id;
                    orderHeader.Status = SD.Status_Approved;
                    await _appDbContext.SaveChangesAsync();

                    RewardDTO reward = new()
                    {
                        UserId = orderHeader.UserId,
                        Email = orderHeader.Email,
                        OrderId = orderHeader.OrderHeaderId,
                        RewardActivity = Convert.ToInt32(orderHeader.TotalAmount)
                    };
                    await _messageBusService
                        .PublishMessage(reward, _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic"));

                    _response.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);
                }
            }
            catch (Exception ex)
            {
                CatchEx(ex);
            }
            return _response;
        }

        private void CatchEx(Exception ex)
        {
            _response.Success = false;
            if(ex.InnerException != null)
            {
                _response.Message = ex.InnerException.Message;
            }
            else
            {
                _response.Message = ex.Message;
            }
        }
    }
}
