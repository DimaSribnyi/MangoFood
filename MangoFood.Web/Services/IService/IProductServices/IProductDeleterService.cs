using MangoFood.Web.Models.DTO;

namespace MangoFood.Web.Services.IService.IProductServices
{
    public interface IProductDeleterService
    {
        Task<ResponseDTO?> DeleteProduct(int id);
    }
}
