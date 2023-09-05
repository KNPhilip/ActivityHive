using API.Dtos;
using API.Services.AuthService;
using Application.Core;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login"), AllowAnonymous]
        public async Task<ActionResult<ServiceResponse<UserDto>>> Login(LoginDto request)
        {
            var response = await _authService.Login(request);
            return response.Success ? Ok(response.Data) : Unauthorized(response.Error ?? "You are not authorized.");
        }

        [HttpPost("register"), AllowAnonymous]
        public async Task<ActionResult<ServiceResponse<UserDto>>> Register(RegisterDto request)
        {
            var response = await _authService.Register(request);
            if (response.Success)
                return Ok(response.Data);
            else 
            {
                ModelState.AddModelError("user", "Email or username already taken.");
                return ValidationProblem();
            }
        }

        [HttpGet, Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var response = await _authService.GetCurrentUser();
            return response.Success ? Ok(response.Data) : NotFound(response.Error);
        }

        [HttpPost("fbLogin"), AllowAnonymous]
        public async Task<ActionResult<UserDto>> FacebookLogin(string accessToken)
        {
            if (!await _authService.VerifyFacebookToken(accessToken))
                return Unauthorized();

            var response = await _authService.FacebookLogin(accessToken);
            return response.Success ? Ok(response.Data) : NotFound(response.Error);
        }
    }
}