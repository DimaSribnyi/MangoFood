using MangoFood.MessageBus;
using MangoFood.Services.AuthAPI.Data;
using MangoFood.Services.AuthAPI.Models;
using MangoFood.Services.AuthAPI.Models.Identites;
using MangoFood.Services.AuthAPI.Services.IServices;
using MangoFood.Services.AuthAPI.Services.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MangoFood.Services.AuthAPI
{
    public static class ServicesExtension
    {
        public static IServiceCollection ConfigureServies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.Configure<JwtOptions>(configuration.GetSection("ApiSettings:JwtOptions"));

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IMessageBusService, MessageBusService>();

            return services;
        }
    }
}
