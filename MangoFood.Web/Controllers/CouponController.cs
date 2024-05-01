using MangoFood.Web.Models.DTO;
using MangoFood.Web.Services.IService.ICouponServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MangoFood.Web.Controllers
{
    [Route("Coupon/[action]")]
    public class CouponController : Controller
    {
        private readonly ICouponAdderService _couponAdderService;
        private readonly ICouponDeleterService _couponDeleterService;
        private readonly ICouponGetterService _couponGetterService;

        public CouponController(ICouponGetterService couponGetterService,
            ICouponDeleterService couponDeleterService,
            ICouponAdderService couponAdderService)
        {
            _couponAdderService = couponAdderService;
            _couponDeleterService = couponDeleterService;
            _couponGetterService = couponGetterService;
        }

        public async Task<IActionResult> CouponIndex()
        {
            var response = await _couponGetterService.GetAllCouponsAsync();

            List<CouponDTO?> coupons = new();

            if(response != null && response.Success)
            {
                coupons = JsonConvert.DeserializeObject<List<CouponDTO>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(coupons);
        }

        public IActionResult CouponCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDTO model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return View(model);
            }

            var response = await _couponAdderService.AddCouponAsync(model);

            if(response != null && response.Success)
            {
                TempData["success"] = "Coupon created successfuly";
                return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(model);
        }

        public async Task<IActionResult> CouponDelete(Guid couponID)
        {
            var response = await _couponGetterService.GetCouponByIDAsync(couponID);
            if( response != null && response.Success )
            {
                var coupon = JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(response.Result));
                return View(coupon);
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return RedirectToAction(nameof(CouponIndex));
        }

        [HttpPost]
        public async Task<IActionResult> CouponDelete(CouponDTO model)
        {
            var response = await _couponDeleterService.DeleteCouponAsync(model.CouponId);

            if (response != null && response.Success)
            {
                TempData["success"] = "Coupon deleted successfuly";
                return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
                return RedirectToAction(nameof(CouponIndex));
            } 
        }
    }
}
