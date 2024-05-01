using MangoFood.Web.Models.DTO;
using MangoFood.Web.Services.IService;
using MangoFood.Web.Services.IService.IProductServices;
using MangoFood.Web.Utility;

namespace MangoFood.Web.Services.Service.ProductServices
{
    public class ProductGetterService : IProductGetterService
    {
        private readonly IBaseService _baseService;

        public ProductGetterService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        
        public async Task<ResponseDTO?> GetProductByID(int id)
        {
            return await _baseService.SendAsync(new()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProductAPIBase + $"/api/product/{id}",
            });
        }

        public async Task<ResponseDTO?> GetProducts()
        {
            return await _baseService.SendAsync(new()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProductAPIBase + $"/api/product",
            });
        }
    }
}
