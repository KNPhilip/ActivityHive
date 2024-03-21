using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API.Dtos;
using API.Services.AuthService;
using Application.Core;
using Infrastructure.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace API.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class AuthController(IAuthService authService, 
        UserManager<User> userManager, EmailSender emailSender) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly UserManager<User> _userManager = userManager;
        private readonly EmailSender _emailSender = emailSender;

        [HttpPost("login"), AllowAnonymous]
        public async Task<ActionResult<ServiceResponse<UserDto>>> Login(LoginDto request)
        {
            User? user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user is null) 
            {
                return Unauthorized();
            }

            ServiceResponse<UserDto?> response = await _authService.Login(request);

            if (response.Success)
            {
                await SetRefreshToken(user);
                return Ok(response.Data);
            }
            else 
            {
                return Unauthorized(response.Error ?? "You are not authorized.");
            }
        }

        [HttpPost("register"), AllowAnonymous]
        public async Task<ActionResult<ServiceResponse<UserDto>>> Register(RegisterDto request)
        {
            ServiceResponse<UserDto?> response = await _authService.Register(request);

            if (response.Success)
            {
                User? user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.UserName == response.Data!.Username);

                if (user is null) 
                {
                    return Unauthorized();
                }
                
                await SetRefreshToken(user);
                return Ok(response.Data);
            }
            else 
            {
                ModelState.AddModelError("user", response.Error);
                return ValidationProblem();
            }
        }

        [HttpGet, Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            User? user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == User.FindFirstValue(ClaimTypes.Email));

            if (user is null) 
            {
                return Unauthorized();
            }

            ServiceResponse<UserDto?> response = await _authService.GetCurrentUser();
            if (response.Success)
            {
                await SetRefreshToken(user!);
                return Ok(response.Data);
            }
            else 
            {
                return NotFound(response.Error);
            }
        }

        [HttpPost("fbLogin"), AllowAnonymous]
        public async Task<ActionResult<UserDto>> FacebookLogin(string accessToken)
        {
            if (!await _authService.VerifyFacebookToken(accessToken)) 
            {
                return Unauthorized();
            }

            ServiceResponse<UserDto?> response = await _authService.FacebookLogin(accessToken);
            return response.Success ? Ok(response.Data) : NotFound(response.Error);
        }

        [HttpPost("refreshToken"), Authorize]
        public async Task<ActionResult<UserDto>> RefreshToken()
        {
            ServiceResponse<UserDto> response = await _authService.RefreshJWT();
            return response.Success ? Ok(response.Data) : Unauthorized(response.Error);
        }

        [HttpPost("verifyEmail"), AllowAnonymous]
        public async Task<IActionResult> VerifyEmail(string token, string email)
        {
            User? user = await _userManager.FindByEmailAsync(email);

            if (user is null) 
            {
                return Unauthorized();
            }

            byte[] decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
            string decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);
            IdentityResult result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded) 
            {
                return BadRequest("Could not verify email address.");
            }

            return Ok("Email confirmed, you can now login");
        }

        [HttpGet("resendEmailConfirmationLink"), AllowAnonymous]
        public async Task<IActionResult> ResendEmailConfirmationLink(string email)
        {
            User? user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return Unauthorized();
            }

            StringValues origin = Request.Headers.Origin;
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            string verifyUrl = $"{origin}/account/verifyEmail?token={token}&email={user.Email}";
            string message = $"<p>Please click the below link to verify your email address:</p><p><a href='{verifyUrl}'>Click to verify email</a></p>";

            await _emailSender.SendEmailAsync(user.Email!, "Please verify email", message);
            
            return Ok("Email verification link resent");
        }

        private async Task SetRefreshToken(User user)
        {
            RefreshToken refreshToken = GenerateRefreshToken();

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            CookieOptions cookieOptions = new()
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }

        private static RefreshToken GenerateRefreshToken() 
        {
            byte[] randomNumber = new byte[32];
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return new RefreshToken{Token = Convert.ToBase64String(randomNumber)};
        }
    }
}
