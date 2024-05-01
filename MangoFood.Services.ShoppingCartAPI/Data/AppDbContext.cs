using MangoFood.Services.ShoppingCartAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MangoFood.Services.ShoppingCartAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<ShoppingCartHeader> ShoppingCartHeaders { get; set; }
        public DbSet<ShoppingCartDetails> ShoppingCartDetails { get; set;}
    }
}
