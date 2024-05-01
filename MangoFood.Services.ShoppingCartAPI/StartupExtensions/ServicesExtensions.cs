using AutoMapper;
using MangoFood.MessageBus;
using MangoFood.Services.ShoppingCartAPI.Data;
using MangoFood.Services.ShoppingCartAPI.Services;
using MangoFood.Services.ShoppingCartAPI.Services.IService;
using MangoFood.Services.ShoppingCartAPI.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace MangoFood.Services.ShoppingCartAPI
{
    public static class ServicesExtensions
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter the Bearer Authorization string",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        }, new string[]{}
                    }
                });
            });

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
            services.AddSingleton(mapper);
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var secret = configuration.GetValue<string>("ApiSettings:Secret");
            var issuer = configuration.GetValue<string>("ApiSettings:Issuer");
            var audience = configuration.GetValue<string>("ApiSettings:Audience");

            var key = Encoding.ASCII.GetBytes(secret!);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience
                };
            });

            services.AddAuthorization();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<IMessageBusService, MessageBusService>();

            services.AddScoped<ApiAuthenticationHttpClientHandler>();
            services.AddHttpContextAccessor();

            services.AddHttpClient("Product",
                c => c.BaseAddress = new Uri(configuration["ServiceUrls:ProductAPI"]))
                .AddHttpMessageHandler<ApiAuthenticationHttpClientHandler>();

            services.AddHttpClient("Coupon",
                c => c.BaseAddress = new Uri(configuration["ServiceUrls:CouponAPI"]))
                .AddHttpMessageHandler<ApiAuthenticationHttpClientHandler>();

            return services;
        }
    }
}
