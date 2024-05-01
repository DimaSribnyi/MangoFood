using MangoFood.Web.Models.DTO;
using MangoFood.Web.Models.DTO.AuthDTO;
using MangoFood.Web.Services.IService;
using MangoFood.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MangoFood.Web.Controllers
{
    [Route("Auth/[action]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService,
            ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.RoleList = roleList;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestDTO registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(registerRequest);
            }

            if (string.IsNullOrEmpty(registerRequest.Role))
            {
                registerRequest.Role = SD.RoleCustomer;
            }

            var response = await _authService.RegisterAsync(registerRequest);
            ResponseDTO? assignRole;

            if (response != null && response.Success)
            {
                assignRole = await _authService.AssignRoleAsync(registerRequest);
                if (assignRole != null && assignRole.Success)
                {
                    TempData["success"] = "Registration is successful";
                    return RedirectToAction(nameof(Login));
                }
            }

            TempData["error"] = response?.Message ?? "Something went wrong";
            ViewBag.RoleList = roleList;
            return View(registerRequest);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(loginRequest);
            }

            var response = await _authService.LoginAsync(loginRequest);

            if (response != null && response.Success)
            {
                var loginResponse = JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(response.Result));

                await SignInUser(loginResponse);
                _tokenProvider.SetToken(loginResponse.Token);

                TempData["success"] = "Login is successfull";
                return RedirectToAction("Index", "Home");
            }

            TempData["error"] = response?.Message ?? "Something went wrong";
            return View(loginRequest);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();

            TempData["success"] = "Log out is successfull";
            return RedirectToAction("Index", "Home");
        }

        private List<SelectListItem> roleList = new List<SelectListItem>()
            {
                new SelectListItem() { Text = SD.RoleCustomer, Value = SD.RoleCustomer},
                new SelectListItem() { Text = SD.RoleAdmin, Value = SD.RoleAdmin }
            };

        private async Task SignInUser(LoginResponseDTO model)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(model.Token);
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));

            identity.AddClaim(new Claim(ClaimTypes.Role,
                jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
