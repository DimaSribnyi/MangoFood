using MangoFood.Web.Models.DTO;
using MangoFood.Web.Models.DTO.CartDTO;
using MangoFood.Web.Models.DTO.OrderDTO;
using MangoFood.Web.Services.IService;
using MangoFood.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace MangoFood.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            var cart = await RetrieveCartByUserId();
            if(cart.CartDetails != null && cart.CartDetails?.First() != null)
            {
                return View(cart);
            }

            TempData["error"] = "Cart is empty";
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var cart = await RetrieveCartByUserId();
            if (cart.CartDetails != null && cart.CartDetails?.First() != null)
            {
                return View(cart);
            }

            TempData["error"] = "Something went wrong";
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(ShoppingCartDTO cartDTO)
        {
            var cart = await RetrieveCartByUserId();
            cart.CartHeader.Email = cartDTO.CartHeader.Email;
            cart.CartHeader.Phone = cartDTO.CartHeader.Phone;
            cart.CartHeader.FirstName = cartDTO.CartHeader.FirstName;
            cart.CartHeader.LastName = cartDTO.CartHeader.LastName;

            var response = await _orderService.CreateOrder(cart);
            var orderHeader = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));

            if(response != null && response.Success)
            {
                var domain = Request.Scheme + "://" + Request.Host + "/";

                StripeRequestDTO stripeRequest = new()
                {
                    ApprovedUrl = domain + "Cart/Confirmation?orderId="+orderHeader.OrderHeaderId,
                    CancelUrl = domain + "Cart/Checkout",
                    OrderHeader = orderHeader
                };

                var stripeResponse = await _orderService.CreateStripeSession(stripeRequest);
                var createdStripe = JsonConvert.DeserializeObject<StripeRequestDTO>(Convert
                    .ToString(stripeResponse.Result));
                
                if(createdStripe == null)
                {
                    TempData["error"] = "Service right now is working incorrectly, sorry";
                    return RedirectToAction("Index", "Home");
                }

                Response.Headers.Add("Location", createdStripe.SessionUrl);
                return new StatusCodeResult(303);
            }

            TempData["error"] = "Cart is empty";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            var response = await _orderService.ValidateStripeSession(orderId);

            if(response != null && response.Success)
            {
                var orderHeader = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));
                if(orderHeader.Status == SD.Status_Approved)
                {
                    return View(orderId);
                }
            }

            TempData["error"] = response?.Message ?? "Something went wrong";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Remove(int detailID)
        {
            var response = await _cartService.RemoveFromCartAsync(detailID);
            if(response != null && response.Success)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            TempData["erorr"] = response?.Message ?? "Something went wrong";
            return RedirectToAction(nameof(CartIndex));
        }

        [HttpPost]
		public async Task<IActionResult> ApplyCoupon(ShoppingCartDTO cartDTO)
        {
            var response = await _cartService.ApplyCouponToCartCartAsync(cartDTO);
            if(response != null && response.Success)
            {
				return RedirectToAction(nameof(CartIndex));
			}

            TempData["error"] = "Coupon is invalid";
			return RedirectToAction(nameof(CartIndex));
		}

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(ShoppingCartDTO cartDTO)
        {
            cartDTO.CartHeader.CouponCode = string.Empty;
            var response = await _cartService.ApplyCouponToCartCartAsync(cartDTO);
            if (response != null && response.Success)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            TempData["error"] = "Something went wrong";
            return RedirectToAction(nameof(CartIndex));
        }

        [HttpPost]
        public async Task<IActionResult> EmailCart()
        {
            var cart = await RetrieveCartByUserId();
            cart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?
                .FirstOrDefault()?.Value;

            var response = await _cartService.EmailCart(cart);
            if (response != null && response.Success)
            {
                TempData["success"] = "Email will be proccessed and sent soon";
                return RedirectToAction(nameof(CartIndex));
            }

            TempData["error"] = "Something went wrong";
            return View(cart);
        }

        private async Task<ShoppingCartDTO?> RetrieveCartByUserId()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            var response = await _cartService.GetCartByUserIdAsync(userId);
            if(response != null && response.Success)
            {
                return JsonConvert.DeserializeObject<ShoppingCartDTO>(Convert.ToString(response.Result));
            }

            return new ShoppingCartDTO();
        }
    }
}
