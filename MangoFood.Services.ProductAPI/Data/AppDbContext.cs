using MangoFood.Services.ProductAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MangoFood.Services.ProductAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        
    }
}
