using MangoFood.Web;
using MangoFood.Web.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

SD.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"]!;
SD.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"]!;
SD.ProductAPIBase = builder.Configuration["ServiceUrls:ProductAPI"]!;
SD.ShoppingCartAPIBase = builder.Configuration["ServiceUrls:ShoppingCartAPI"]!;
SD.OrderAPIBase = builder.Configuration["ServiceUrls:OrderAPI"]!;

builder.Services.ConfigureServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(new ExceptionHandlerOptions()
    {
        AllowStatusCode404Response = true,
        ExceptionHandlingPath = "/error"
    });
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
