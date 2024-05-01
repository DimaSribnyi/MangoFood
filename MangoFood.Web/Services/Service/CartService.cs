using MangoFood.Web.Models.DTO;
using MangoFood.Web.Models.DTO.CartDTO;
using MangoFood.Web.Services.IService;
using MangoFood.Web.Utility;

namespace MangoFood.Web.Services.Service
{
    public class CartService : ICartService
    {
        private readonly IBaseService _baseService;

        public CartService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO?> ApplyCouponToCartCartAsync(ShoppingCartDTO cartDTO)
        {
            return await _baseService.SendAsync(new() 
            { 
                ApiType = SD.ApiType.POST,
                Url = SD.ShoppingCartAPIBase + $"/api/Cart/ApplyCoupon",
                Data = cartDTO
            });

        }

        public async Task<ResponseDTO?> EmailCart(ShoppingCartDTO cartDTO)
        {
            return await _baseService.SendAsync(new()
            {
                ApiType= SD.ApiType.POST,
                Url = SD.ShoppingCartAPIBase + $"/api/Cart/EmailCartRequest",
                Data = cartDTO
            });
        }

        public async Task<ResponseDTO?> GetCartByUserIdAsync(string userID)
        {
            return await _baseService.SendAsync(new()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ShoppingCartAPIBase + $"/api/Cart/GetCart/{userID}"
            });
        }

        public async Task<ResponseDTO?> RemoveFromCartAsync(int cardDetailsId)
        {
            return await _baseService.SendAsync(new()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.ShoppingCartAPIBase + $"/api/Cart/RemoveCart",
                Data = cardDetailsId
            });
        }

        public async Task<ResponseDTO?> UpsertCartAsync(ShoppingCartDTO cartDTO)
        {
            return await _baseService.SendAsync(new()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.ShoppingCartAPIBase + $"/api/Cart/UpsertCart",
                Data = cartDTO
            });
        }
    }
}
