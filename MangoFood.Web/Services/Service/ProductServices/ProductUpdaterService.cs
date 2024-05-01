
using MangoFood.Web.Models.DTO;
using MangoFood.Web.Services.IService;
using MangoFood.Web.Services.IService.IProductServices;
using MangoFood.Web.Utility;

namespace MangoFood.Web.Services.Service.ProductServices
{
    public class ProductUpdaterService : IProductUpdaterService
    {
        private readonly IBaseService _baseService;

        public ProductUpdaterService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO?> UpdateProduct(ProductDTO productDTO, int id)
        {
            return await _baseService.SendAsync(new()
            {
                ApiType = SD.ApiType.PUT,
                Url = SD.ProductAPIBase + $"/api/product/{id}",
                Data = productDTO,
                ConentType = SD.ConentType.MultipartFormData
            });
        }
    }
}
