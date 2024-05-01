using MangoFood.Services.AuthAPI.Models.Identites;

namespace MangoFood.Services.AuthAPI.Services.IServices
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
    }
}
