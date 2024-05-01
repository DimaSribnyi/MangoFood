using MangoFood.Services.AuthAPI.Models.DTO;

namespace MangoFood.Services.AuthAPI.Services.IServices
{
    public interface IAuthService
    {
        Task<string> Register(RegisterRequestDTO registerRequest); 
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest);
        Task<bool> AssignRole(string email, string roleName);
    }
}
