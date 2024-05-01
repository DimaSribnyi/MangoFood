using MangoFood.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MangoFood.Services.RewardAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Reward> Rewards { get; set; }
    }
}
