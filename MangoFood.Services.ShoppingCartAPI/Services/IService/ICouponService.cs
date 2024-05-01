using MangoFood.Services.ShoppingCartAPI.Models.DTO;

namespace MangoFood.Services.ShoppingCartAPI.Services.IService
{
    public interface ICouponService
    {
        Task<CouponDTO> GetCoupon(string couponCode);
    }
}
