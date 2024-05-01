using MangoFood.Web.Models.DTO;
using MangoFood.Web.Services.IService;
using MangoFood.Web.Services.IService.ICouponServices;
using MangoFood.Web.Utility;

namespace MangoFood.Web.Services.Service.CouponServices
{
    public class CouponAdderService : ICouponAdderService
    {
        private readonly IBaseService _baseService;

        public CouponAdderService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO?> AddCouponAsync(CouponDTO couponDTO)
        {
            return await _baseService.SendAsync(new()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.CouponAPIBase + $"/api/Coupon",
                Data = couponDTO
            });
        }
    }
}
