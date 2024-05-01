using MangoFood.Services.CouponAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MangoFood.Services.CouponAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponId = Guid.NewGuid(),
                CouponCode = "10OFF",
                DiscountAmount = 10,
                MinAmount = 20
            });

            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponId = Guid.NewGuid(),
                CouponCode = "20OFF",
                DiscountAmount = 20,
                MinAmount = 40
            });

            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponId = Guid.NewGuid(),
                CouponCode = "30OFF",
                DiscountAmount = 30,
                MinAmount = 60
            });

            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponId = Guid.NewGuid(),
                CouponCode = "54OFF",
                DiscountAmount = 54,
                MinAmount = 100
            });
        }
    }
}
