
using MangoFood.Web.Models.DTO;

namespace MangoFood.Web.Services.IService.IProductServices
{
    public interface IProductAdderService
    {
        Task<ResponseDTO?> AddProduct(ProductDTO productDTO);
    }
}
