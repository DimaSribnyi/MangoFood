using MangoFood.Web.Models.DTO;

namespace MangoFood.Web.Services.IService.ICouponServices
{
    public interface ICouponGetterService
    {
        Task<ResponseDTO?> GetCouponAsync(string couponCode);
        Task<ResponseDTO?> GetAllCouponsAsync();
        Task<ResponseDTO?> GetCouponByIDAsync(Guid id);
    }
}
