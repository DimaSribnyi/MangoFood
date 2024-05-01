using MangoFood.Web.Models.DTO;
using MangoFood.Web.Models.DTO.CartDTO;

namespace MangoFood.Web.Services.IService
{
    public interface ICartService
    {
        Task<ResponseDTO?> GetCartByUserIdAsync(string userID);
        Task<ResponseDTO?> UpsertCartAsync(ShoppingCartDTO cartDTO);
        Task<ResponseDTO?> RemoveFromCartAsync(int cardDetailsId);
        Task<ResponseDTO?> ApplyCouponToCartCartAsync(ShoppingCartDTO cartDTO);
        Task<ResponseDTO?> EmailCart(ShoppingCartDTO cartDTO);
    }
}
