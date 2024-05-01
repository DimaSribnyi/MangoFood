using MangoFood.Web.Models.DTO;

namespace MangoFood.Web.Services.IService.ICouponServices
{
    public interface ICouponDeleterService
    {
        Task<ResponseDTO?> DeleteCouponAsync(Guid id);
    }
}
