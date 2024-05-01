using MangoFood.Services.RewardAPI.Data;
using MangoFood.Services.RewardAPI.Models;
using MangoFood.Services.RewardAPI.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace MangoFood.Services.RewardAPI.Services
{
    public class RewardService : IRewardService
    {
        private DbContextOptions<AppDbContext> options;

        public RewardService(DbContextOptions<AppDbContext> options)
        {
            this.options = options;
        }

        public async Task UpdateRewards(RewardDTO rewardDTO)
        {
            try
            {
                Reward reward = new()
                {
                    OrderId = rewardDTO.OrderId,
                    RewardsActivity = rewardDTO.RewardActivity,
                    UserId = rewardDTO.UserId,
                    RewardsDate = DateTime.Now
                };
                await using var _db = new AppDbContext(options);
                _db.Rewards.Add(reward);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;   
            }
        }
    }
}
