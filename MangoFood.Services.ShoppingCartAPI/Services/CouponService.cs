using MangoFood.Services.ShoppingCartAPI.Models.DTO;
using MangoFood.Services.ShoppingCartAPI.Services.IService;
using Newtonsoft.Json;

namespace MangoFood.Services.ShoppingCartAPI.Services
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CouponService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<CouponDTO> GetCoupon(string couponCode)
        {
            var client = _httpClientFactory.CreateClient("Coupon");
            var response = await client.GetAsync($"/api/coupon/{couponCode}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var responseDTO = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
            if(responseDTO != null && responseDTO.Success)
            {
                return JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(responseDTO.Result));
            }

            return new CouponDTO();
        }
    }
}
