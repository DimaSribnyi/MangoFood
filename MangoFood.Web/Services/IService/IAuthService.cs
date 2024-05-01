using MangoFood.Web.Models.DTO;
using MangoFood.Web.Models.DTO.AuthDTO;

namespace MangoFood.Web.Services.IService
{
    public interface IAuthService
    {
        Task<ResponseDTO?> LoginAsync(LoginRequestDTO loginRequest);
        Task<ResponseDTO?> RegisterAsync(RegisterRequestDTO registerRequest);
        Task<ResponseDTO?> AssignRoleAsync(RegisterRequestDTO registerRequest);
    }
}
