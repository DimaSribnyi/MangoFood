using MangoFood.Web.Models.DTO;
using MangoFood.Web.Services.IService;
using MangoFood.Web.Services.IService.IProductServices;
using MangoFood.Web.Utility;

namespace MangoFood.Web.Services.Service.ProductServices
{
    public class ProductDeleterService : IProductDeleterService
    {
        private readonly IBaseService _baseService;

        public ProductDeleterService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO?> DeleteProduct(int id)
        {
            return await _baseService.SendAsync(new()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.ProductAPIBase + $"/api/product/{id}",
            });
        }
    }
}
