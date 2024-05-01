using MangoFood.Web.Models.DTO;

namespace MangoFood.Web.Services.IService.IProductServices
{
    public interface IProductUpdaterService
    {
        Task<ResponseDTO?> UpdateProduct(ProductDTO productDTO, int id);
    }
}
