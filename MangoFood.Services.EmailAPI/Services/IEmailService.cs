using MangoFood.Services.EmailAPI.Models.DTO;

namespace MangoFood.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailAndCatalog(ShoppingCartDTO cartDTO);
        Task UserRegisteredCatalog(string email);  
        Task LogOrderPlaced(RewardDTO rewardDTO);
    }
}
