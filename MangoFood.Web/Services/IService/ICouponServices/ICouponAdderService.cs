using MangoFood.Web.Models.DTO;

namespace MangoFood.Web.Services.IService.ICouponServices
{
    public interface ICouponAdderService
    {
        Task<ResponseDTO?> AddCouponAsync(CouponDTO couponDTO);
    }
}
