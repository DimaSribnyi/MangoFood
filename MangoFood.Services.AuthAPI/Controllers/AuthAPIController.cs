using MangoFood.MessageBus;
using MangoFood.Services.AuthAPI.Models.DTO;
using MangoFood.Services.AuthAPI.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace MangoFood.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private ResponseDTO _response;
        private readonly IAuthService _authService;
        private readonly IMessageBusService _messageBusService;
        private readonly IConfiguration _configuration;

        public AuthAPIController(IAuthService authService, IConfiguration configuration, 
            IMessageBusService messageBusService)
        {
            _authService = authService;
            _response = new ResponseDTO();
            _configuration = configuration;
            _messageBusService = messageBusService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDTO registerRequest)
        {
            string errorMassage = await _authService.Register(registerRequest);
            if(!string.IsNullOrEmpty(errorMassage))
            {
                _response.Success = false;
                _response.Message = errorMassage;
                return BadRequest(_response);
            }

            await _messageBusService
                   .PublishMessage(registerRequest.Email,
                   _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequest)
        {
            var loginResponse = await _authService.Login(loginRequest);
            if(loginResponse.User == null)
            {
                _response.Success = false;
                _response.Message = "Wrong email or password";
                return BadRequest(_response);
            } 
            
            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole(RegisterRequestDTO registerRequest)
        {
            if(await _authService.AssignRole(registerRequest.Email, registerRequest.Role))
            {
                return Ok(_response);
            }

            _response.Success = false;
            _response.Message = "Error encountered";
            return BadRequest(_response);
        }
    }
}
