
using MangoFood.Services.RewardAPI.Models.DTO;

namespace MangoFood.Services.RewardAPI.Services
{
    public interface IRewardService
    {
        Task UpdateRewards(RewardDTO reward);
    }
}
