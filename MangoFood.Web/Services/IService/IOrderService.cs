using MangoFood.Web.Models.DTO;
using MangoFood.Web.Models.DTO.CartDTO;

namespace MangoFood.Web.Services.IService
{
    public interface IOrderService
    {
        Task<ResponseDTO?> CreateOrder(ShoppingCartDTO cartDTO);
        Task<ResponseDTO?> CreateStripeSession(StripeRequestDTO stripeRequest);
        Task<ResponseDTO?> ValidateStripeSession(int orderHeaderId);
        Task<ResponseDTO?> GetOrders(string userId);
        Task<ResponseDTO?> GetOrderById(int id);
        Task<ResponseDTO?> UpdateOrderStatus(int orderId, string newStatus);
    }
}
