using MangoFood.Services.AuthAPI.Data;
using MangoFood.Services.AuthAPI.Models.DTO;
using MangoFood.Services.AuthAPI.Models.Identites;
using MangoFood.Services.AuthAPI.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace MangoFood.Services.AuthAPI.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(UserManager<ApplicationUser> userManager,
            AppDbContext appDbContext, RoleManager<IdentityRole> roleManager,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = await _userManager.FindByNameAsync(email);
            if(user != null)
            {
                if(!(await _roleManager.RoleExistsAsync(roleName)))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }

                await _userManager.AddToRoleAsync(user, roleName);  
                return true;
            }

            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest)
        {

            var user = await _userManager.FindByNameAsync(loginRequest.UserName);

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

            if (user is null || !isValid)
            {
                return new LoginResponseDTO() { User = null, Token = "" };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            UserDTO userDTO = new()
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            LoginResponseDTO responseDTO = new()
            {
                User = userDTO,
                Token = token
            };

            return responseDTO;

        }

        public async Task<string> Register(RegisterRequestDTO registerRequest)
        {
            ApplicationUser user = new()
            {
                UserName = registerRequest.Email,
                Email = registerRequest.Email,
                NormalizedEmail = registerRequest.Email.ToLower(),
                PhoneNumber = registerRequest.PhoneNumber,
                Name = registerRequest.Name,
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registerRequest.Password);
                if (result.Succeeded)
                {
                    return string.Empty;
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return ex.InnerException.Message;
                }
                return ex.Message;
            }
        }
    }
}
