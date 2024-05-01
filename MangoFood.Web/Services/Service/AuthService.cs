using MangoFood.Web.Models.DTO;
using MangoFood.Web.Models.DTO.AuthDTO;
using MangoFood.Web.Services.IService;
using MangoFood.Web.Utility;

namespace MangoFood.Web.Services.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;

        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO?> AssignRoleAsync(RegisterRequestDTO registerRequest)
        {
            return await _baseService.SendAsync(new()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.AuthAPIBase + $"/api/auth/assignrole",
                Data = registerRequest
            }, withBearer:false);
        }

        public async Task<ResponseDTO?> LoginAsync(LoginRequestDTO loginRequest)
        {
            return await _baseService.SendAsync(new()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.AuthAPIBase + $"/api/auth/login",
                Data = loginRequest
            }, withBearer: false);
        }

        public async Task<ResponseDTO?> RegisterAsync(RegisterRequestDTO registerRequest)
        {
            return await _baseService.SendAsync(new()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.AuthAPIBase + $"/api/auth/register",
                Data = registerRequest
            }, withBearer: false);
        }
    }
}
