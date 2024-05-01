using MangoFood.Web.Models.DTO;

namespace MangoFood.Web.Services.IService.IProductServices
{
    public interface IProductGetterService
    {
        Task<ResponseDTO?> GetProducts();
        Task<ResponseDTO?> GetProductByID(int id);
    }
}
