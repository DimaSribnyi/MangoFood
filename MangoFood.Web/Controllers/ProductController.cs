using MangoFood.Web.Models.DTO;
using MangoFood.Web.Services.IService.IProductServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MangoFood.Web.Controllers
{
	[Route("product/[action]")]
	public class ProductController : Controller
	{
		private readonly IProductGetterService _productGetterService;
		private readonly IProductAdderService _productAdderService;
		private readonly IProductUpdaterService _productUpdaterService;
		private readonly IProductDeleterService _productDeleterService;

		public ProductController(IProductGetterService productGetterService,
			IProductAdderService productAdderService,
			IProductUpdaterService productUpdaterService,
			IProductDeleterService productDeleterService)
		{
			_productGetterService = productGetterService;
			_productAdderService = productAdderService;
			_productUpdaterService = productUpdaterService;
			_productDeleterService = productDeleterService;
		}

		public async Task<IActionResult> ProductIndex()
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

		public IActionResult CreateProduct()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateProduct(ProductDTO model)
		{
			if (!ModelState.IsValid)
			{
				TempData["error"] = ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage;
				return View(model);
			}

			var response = await _productAdderService.AddProduct(model);
			if (response != null && response.Success)
			{
				TempData["success"] = "Product added successfully";
				return RedirectToAction(nameof(ProductIndex));
			}

			TempData["error"] = response?.Message ?? "Something went wrong";
			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> EditProduct(int id)
		{
			var response = await _productGetterService.GetProductByID(id);
			if (response != null && response.Success)
			{
				var product = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
				return View(product);
			}

			TempData["error"] = response?.Message ?? "Something went wrong";
			return RedirectToAction(nameof(ProductIndex));
		}

		[HttpPost]
		public async Task<IActionResult> EditProduct(ProductDTO model)
		{
			if (!ModelState.IsValid)
			{
				TempData["error"] = ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage;
				return View(model);
			}

			var response = await _productUpdaterService.UpdateProduct(model, model.ProductId);
			if (response != null && response.Success)
			{
				TempData["success"] = "Updation is successfull";
				return RedirectToAction(nameof(ProductIndex));
			}

			TempData["error"] = response?.Message ?? "Something went wrong";
			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> DeleteProduct(int id)
		{
			var response = await _productGetterService.GetProductByID(id);
			if (response != null && response.Success)
			{
				var product = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
				return View(product);
			}

			TempData["error"] = response?.Message ?? "Something went wrong";
			return RedirectToAction(nameof(ProductIndex));
		}

		[HttpPost]
		public async Task<IActionResult> DeleteProduct(ProductDTO model)
		{
			var response = await _productDeleterService.DeleteProduct(model.ProductId);
			if (response != null && response.Success)
			{
				TempData["success"] = "Deletion is successfull";
				return RedirectToAction(nameof(ProductIndex));
			}

			TempData["error"] = response?.Message ?? "Something went wrong";
			return RedirectToAction(nameof(DeleteProduct), new {id = model.ProductId });
		}
	}
}
