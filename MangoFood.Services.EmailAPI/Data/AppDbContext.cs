
using MangoFood.Services.EmailAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MangoFood.Services.ShoppingCartAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<EmailLogger> EmailLoggers { get; set; }
    }
}
