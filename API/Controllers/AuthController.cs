using API.Dtos;
using API.Services.AuthService;
using Application.Core;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [ApiController, Route("api/[controller]"), AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<UserDto>>> Login(LoginDto request)
        {
            var response = await _authService.Login(request);
            return response.Success ? Ok(response.Data) : Unauthorized(response!.Error ?? "You are not authorized.");
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<UserDto>>> Register(RegisterDto request)
        {
            var response = await _authService.Register(request);
            return response.Success ? Ok(response.Data) : BadRequest(response!.Error);
        }
    }
}