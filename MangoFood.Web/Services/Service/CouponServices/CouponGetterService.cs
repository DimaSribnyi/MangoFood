using MangoFood.Web.Models.DTO;
using MangoFood.Web.Services.IService;
using MangoFood.Web.Services.IService.ICouponServices;
using MangoFood.Web.Utility;

namespace MangoFood.Web.Services.Service.CouponServices
{
    public class CouponGetterService : ICouponGetterService
    {
        private readonly IBaseService _baseService;

        public CouponGetterService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO?> GetAllCouponsAsync()
        {
            return await _baseService.SendAsync(new()
            {
                ApiType =SD.ApiType.GET,
                Url = SD.CouponAPIBase + "/api/Coupon"
            });
        }

        public async Task<ResponseDTO?> GetCouponAsync(string couponCode)
        {
            return await _baseService.SendAsync(new()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponAPIBase + $"/api/Coupon/{couponCode}"
            });
        }

        public async Task<ResponseDTO?> GetCouponByIDAsync(Guid id)
        {
            return await _baseService.SendAsync(new()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponAPIBase + $"/api/Coupon/{id}"
            });
        }
    }
}
