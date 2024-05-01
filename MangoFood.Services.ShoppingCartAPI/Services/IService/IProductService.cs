

using MangoFood.Services.ShoppingCartAPI.Models.DTO;

namespace MangoFood.Services.ShoppingCartAPI.Services.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProducts();
    }
}
