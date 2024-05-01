using MangoFood.Web.Models.DTO;
using MangoFood.Web.Services.IService;
using MangoFood.Web.Services.IService.ICouponServices;
using MangoFood.Web.Utility;

namespace MangoFood.Web.Services.Service.CouponServices
{
    public class CouponDeleterService : ICouponDeleterService
    {
        private readonly IBaseService _baseService;

        public CouponDeleterService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO?> DeleteCouponAsync(Guid id)
        {
            return await _baseService.SendAsync(new()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.CouponAPIBase + $"/api/Coupon/{id}"
            });
        }
    }
}
