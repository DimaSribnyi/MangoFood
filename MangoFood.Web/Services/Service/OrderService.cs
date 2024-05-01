using MangoFood.Web.Models.DTO;
using MangoFood.Web.Models.DTO.CartDTO;
using MangoFood.Web.Services.IService;
using MangoFood.Web.Utility;

namespace MangoFood.Web.Services.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;

        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO?> CreateOrder(ShoppingCartDTO cartDTO)
        {
            return await _baseService.SendAsync(new()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.OrderAPIBase + $"/api/Order/CreateOrder",
                Data = cartDTO
            });
        }

        public async Task<ResponseDTO?> CreateStripeSession(StripeRequestDTO stripeRequest)
        {
            return await _baseService.SendAsync(new()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.OrderAPIBase + "/api/Order/CreateStripeSession",
                Data = stripeRequest
            });
        }

        public async Task<ResponseDTO?> GetOrderById(int id)
        {
            return await _baseService.SendAsync(new()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIBase + $"/api/Order/GetOrder/{id}"
            });
        }

        public async Task<ResponseDTO?> GetOrders(string userId)
        {
            return await _baseService.SendAsync(new()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIBase + $"/api/Order/GetOrders?userId={userId}"
            });
        }

        public async Task<ResponseDTO?> UpdateOrderStatus(int orderId, string newStatus)
        {
            return await _baseService.SendAsync(new()
            {
                ApiType = SD.ApiType.PUT,
                Url = SD.OrderAPIBase + $"/api/Order/UpdateOrderStatus/{orderId}",
                Data = newStatus,
            });
        }

        public async Task<ResponseDTO?> ValidateStripeSession(int orderHeaderId)
        {
            return await _baseService.SendAsync(new()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.OrderAPIBase + "/api/Order/ValidateStripeSession",
                Data = orderHeaderId
            });
        }
    }
}
