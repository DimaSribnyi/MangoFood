using MangoFood.Web.Services.IService;
using MangoFood.Web.Services.IService.ICouponServices;
using MangoFood.Web.Services.IService.IProductServices;
using MangoFood.Web.Services.Service;
using MangoFood.Web.Services.Service.CouponServices;
using MangoFood.Web.Services.Service.ProductServices;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MangoFood.Web
{
    public static class ServicesExtensions
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddHttpContextAccessor();
            services.AddHttpClient();

            services.AddHttpClient<ICouponAdderService, CouponAdderService>();
            services.AddHttpClient<ICouponGetterService, CouponGetterService>();
            services.AddHttpClient<ICouponDeleterService, CouponDeleterService>();

            services.AddHttpClient<IProductAdderService, ProductAdderService>();
            services.AddHttpClient<IProductDeleterService, ProductDeleterService>();
            services.AddHttpClient<IProductGetterService, ProductGetterService>();
            services.AddHttpClient<IProductUpdaterService, ProductUpdaterService>();

            services.AddHttpClient<IAuthService, AuthService>();
            services.AddHttpClient<ICartService, CartService>();
            services.AddHttpClient<IOrderService, OrderService>();

            services.AddScoped<IBaseService, BaseService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenProvider, TokenProvider>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();

            services.AddScoped<ICouponAdderService, CouponAdderService>();
            services.AddScoped<ICouponGetterService, CouponGetterService>();
            services.AddScoped<ICouponDeleterService, CouponDeleterService>();

            services.AddScoped<IProductAdderService, ProductAdderService>();
            services.AddScoped<IProductDeleterService, ProductDeleterService>();
            services.AddScoped<IProductGetterService, ProductGetterService>();
            services.AddScoped<IProductUpdaterService, ProductUpdaterService>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.AccessDeniedPath = "/Auth/AccessDenied";
                options.LoginPath = "/Auth/Login";
                options.ExpireTimeSpan = TimeSpan.FromHours(10);
            });

            return services;
        }
    }
}
