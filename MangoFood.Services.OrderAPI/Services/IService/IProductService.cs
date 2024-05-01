


using MangoFood.Services.OrderAPI.Models.DTO;

namespace MangoFood.Services.OrderAPI.Services.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProducts();
    }
}
