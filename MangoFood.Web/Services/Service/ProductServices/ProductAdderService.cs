
using MangoFood.Web.Models.DTO;
using MangoFood.Web.Services.IService;
using MangoFood.Web.Services.IService.IProductServices;
using MangoFood.Web.Utility;

namespace MangoFood.Web.Services.Service.ProductServices
{
    public class ProductAdderService : IProductAdderService
    {
        private readonly IBaseService _baseService;

        public ProductAdderService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO?> AddProduct(ProductDTO productDTO)
        {
            return await _baseService.SendAsync(new()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.ProductAPIBase + $"/api/product",
                Data = productDTO,
                ConentType = SD.ConentType.MultipartFormData
            });
        }
    }
}
