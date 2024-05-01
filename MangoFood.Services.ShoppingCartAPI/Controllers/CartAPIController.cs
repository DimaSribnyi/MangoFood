using AutoMapper;
using MangoFood.MessageBus;
using MangoFood.Services.ShoppingCartAPI.Data;
using MangoFood.Services.ShoppingCartAPI.Models;
using MangoFood.Services.ShoppingCartAPI.Models.DTO;
using MangoFood.Services.ShoppingCartAPI.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MangoFood.Services.ShoppingCartAPI.Controllers
{
    [Route("api/Cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IMessageBusService _messageBusService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private ResponseDTO _response;

        public CartAPIController(AppDbContext appDbContext, IMapper mapper,
            IProductService productService, ICouponService couponService,
            IMessageBusService messageBusService, IConfiguration configuration)
        {
            _appDbContext = appDbContext;
            _productService = productService;
            _messageBusService = messageBusService;
            _response = new ResponseDTO();
            _couponService = couponService;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPost("UpsertCart")]
        public async Task<ActionResult<ResponseDTO>> UpsertCart(ShoppingCartDTO cartDTO)
        {
            try
            {
                var cartHeaderFromDb = await _appDbContext.ShoppingCartHeaders.AsNoTracking()
                    .FirstOrDefaultAsync(h => h.UserId == cartDTO.CartHeader.UserId);

                if (cartHeaderFromDb == null)
                {
                    //create cart
                    var cartHedaer = _mapper.Map<ShoppingCartHeader>(cartDTO.CartHeader);
                    _appDbContext.Add(cartHedaer);
                    await _appDbContext.SaveChangesAsync();

                    cartDTO.CartDetails.First().CartHeaderId = cartHedaer.CartHeaderId;
                    _appDbContext.ShoppingCartDetails.Add(_mapper.Map<ShoppingCartDetails>(cartDTO.CartDetails.First()));
                    await _appDbContext.SaveChangesAsync();
                }
                else
                {
                    //check if details has the same product
                    var cartDetailsFromDb = await _appDbContext.ShoppingCartDetails.AsNoTracking()
                        .FirstOrDefaultAsync(d => d.ProductId == cartDTO.CartDetails.First().ProductId &&
                        cartHeaderFromDb.CartHeaderId == d.CartHeaderId);

                    if (cartDetailsFromDb == null)
                    {
                        //create cart details of a product
                        cartDTO.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        _appDbContext.ShoppingCartDetails.Add(_mapper.Map<ShoppingCartDetails>(cartDTO.CartDetails.First()));
                        await _appDbContext.SaveChangesAsync();
                    }
                    else
                    {
                        //update count
                        cartDTO.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDTO.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDTO.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        _appDbContext.Update(_mapper.Map<ShoppingCartDetails>(cartDTO.CartDetails.First()));
                        await _appDbContext.SaveChangesAsync();
                    }
                }

                _response.Result = cartDTO;
            }
            catch (Exception ex)
            {
                CatchEx(ex);
            }

            return _response;
        }

        [HttpPost("RemoveCart")]
        public async Task<ActionResult<ResponseDTO>> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                var cartDetails = await _appDbContext.ShoppingCartDetails
                    .FirstOrDefaultAsync(d => d.CartDetailsId == cartDetailsId);

                if (cartDetails == null)
                {
                    _response.Success = false;
                    return NotFound(_response);
                }

                int countOfCardItems = await _appDbContext.ShoppingCartDetails
                    .Where(d => d.CartHeaderId == cartDetails.CartHeaderId).CountAsync();

                _appDbContext.ShoppingCartDetails.Remove(cartDetails);
                if (countOfCardItems == 1)
                {
                    var cartHeader = await _appDbContext.ShoppingCartHeaders
                        .FirstAsync(h => h.CartHeaderId == cartDetails.CartHeaderId);
                    _appDbContext.ShoppingCartHeaders.Remove(cartHeader);
                }

                await _appDbContext.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                CatchEx(ex);
            }

            return _response;
        }

        [HttpGet("GetCart/{userID}")]
        public async Task<ActionResult<ResponseDTO>> GetShoppingCart(string userID)
        {
            try
            {
                ShoppingCartDTO shoppingCart = new()
                {
                    CartHeader = _mapper.Map<ShoppingCartHeaderDTO>(await _appDbContext.ShoppingCartHeaders
                        .FirstAsync(h => h.UserId == userID))
                };

                shoppingCart.CartDetails = _mapper
                    .Map<IEnumerable<ShoppingCartDetailsDTO>>(_appDbContext.ShoppingCartDetails
                    .Where(d => d.CartHeaderId == shoppingCart.CartHeader.CartHeaderId));

                var products = await _productService.GetProducts();

                foreach (var item in shoppingCart.CartDetails)
                {
                    item.Product = products.FirstOrDefault(p => p.ProductId == item.ProductId);

                    shoppingCart.CartHeader.TotalAmount += item.Count * item.Product.Price;

                }

                shoppingCart.CartHeader.TotalAmount = Math.Round(shoppingCart.CartHeader.TotalAmount);

                if (!string.IsNullOrEmpty(shoppingCart.CartHeader.CouponCode))
                {
                    var coupon = await _couponService.GetCoupon(shoppingCart.CartHeader.CouponCode);
                    if (coupon != null && shoppingCart.CartHeader.TotalAmount >= coupon.MinAmount)
                    {
                        shoppingCart.CartHeader.Discount = coupon.DiscountAmount;
                        shoppingCart.CartHeader.TotalAmount -= coupon.DiscountAmount;
                    }
                }

                _response.Result = shoppingCart;
            }
            catch (Exception ex)
            {
                CatchEx(ex);
            }

            return _response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<ActionResult<ResponseDTO>> ApplyCoupon(ShoppingCartDTO cartDTO)
        {
            try
            {
                var cartFromDb = await _appDbContext.ShoppingCartHeaders.AsNoTracking()
                    .FirstAsync(d => d.CartHeaderId == cartDTO.CartHeader.CartHeaderId);
                cartFromDb.CouponCode = cartDTO.CartHeader.CouponCode;
                _appDbContext.Update(cartFromDb);
                await _appDbContext.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                CatchEx(ex);
            }

            return _response;
        }

        [HttpPost("EmailCartRequest")]
        public async Task<ActionResult<ResponseDTO>> EmailCartRequest(ShoppingCartDTO cartDTO)
        {
            try
            {
                var topicName = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
                await _messageBusService.PublishMessage(cartDTO, topicName!);
                _response.Result = true;
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
            if (ex.InnerException != null)
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
