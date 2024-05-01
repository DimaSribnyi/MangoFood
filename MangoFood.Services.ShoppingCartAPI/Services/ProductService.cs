using MangoFood.Services.ShoppingCartAPI.Models.DTO;
using MangoFood.Services.ShoppingCartAPI.Services.IService;
using Newtonsoft.Json;


namespace MangoFood.Services.ShoppingCartAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<ProductDTO>> GetProducts()
        {
            var client = _httpClientFactory.CreateClient("Product");
            var response = await client.GetAsync($"/api/product");
            var apiContent = await response.Content.ReadAsStringAsync();
            var responseDTO = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);

            if(responseDTO != null && responseDTO.Success)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDTO>>(Convert
                    .ToString(responseDTO.Result));
            }
            return new List<ProductDTO>();  
        }
    }
}
