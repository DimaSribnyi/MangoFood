using MangoFood.Web.Models;
using MangoFood.Web.Models.DTO;
using MangoFood.Web.Models.DTO.CartDTO;
using MangoFood.Web.Services.IService;
using MangoFood.Web.Services.IService.IProductServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace MangoFood.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly IProductGetterService _productGetterService;
        private readonly ICartService _cartService;

        public HomeController(IProductGetterService productGetterService, ICartService cartService)
        {
            _productGetterService = productGetterService;
            _cartService = cartService;
        }

        [Route("/")]
        public async Task<IActionResult> Index()
        {
            var response = await _productGetterService.GetProducts();

            List<ProductDTO> products = new();
            if (response != null && response.Success)
            {
                products = JsonConvert.DeserializeObject<List<ProductDTO>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message ?? "Something went wrong";
            }

            return View(products);
        }

        [Authorize]
        public async Task<IActionResult> ProductDetails(int productID)
        {
            var response = await _productGetterService.GetProductByID(productID);
            ProductDTO product = new();

            if (response != null && response.Success)
            {
                product = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message ?? "Something went wrong";
            }

            return View(product);
        }

        [Authorize]
        [HttpPost]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDTO productDTO)
        {
            ShoppingCartDTO cartDTO = new ShoppingCartDTO()
            {
                CartHeader = new ShoppingCartHeaderDTO()
                {
                    UserId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value
                },
            };

            cartDTO.CartDetails = new List<ShoppingCartDetailsDTO>()
            {
                new ShoppingCartDetailsDTO()
                    {
                        ProductId = productDTO.ProductId,
                        Count = productDTO.Count,
                    }
            };

            var response = await _cartService.UpsertCartAsync(cartDTO);

            if (response != null && response.Success)
            {
                TempData["success"] = $"Product added to cart successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.Message ?? "Something went wrong";
            }

            return View(productDTO);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
