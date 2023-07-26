using API.Dtos;
using API.Services.AuthService;
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
        public async Task<ActionResult<UserDto>> Login(LoginDto request)
        {
            var result = await _authService.Login(request);
            return result is null ? Unauthorized() : Ok(result);
        }
    }
}