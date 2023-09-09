using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API.Dtos;
using Application.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;

        public AuthService(UserManager<User> userManager, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://graph.facebook.com")
            };
        }

        public string CreateJWT(User user)
        {
            List<Claim> claims = new() 
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<ServiceResponse<UserDto?>> Login(LoginDto request)
        {
            User? user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null) 
                return new ServiceResponse<UserDto?> { Error = "Incorrect email or password." };

            bool result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (result)
            {
                await SetRefreshToken(user);
                UserDto returningUser = CreateUserObject(user);
                return ServiceResponse<UserDto?>.SuccessResponse(returningUser);
            }
            return new ServiceResponse<UserDto?> { Error = "Incorrect email or password." };
        }

        public async Task<ServiceResponse<UserDto?>> Register(RegisterDto request)
        {
            if (await _userManager.Users.AnyAsync(x => x.UserName == request.Username))
                return new ServiceResponse<UserDto?> { Error = "This username is already taken.." };

            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email))
                return new ServiceResponse<UserDto?> { Error = "Email is already taken.." };

            User user = new() 
            {
                DisplayName = request.DisplayName,
                Email = request.Email,
                UserName = request.Username,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await SetRefreshToken(user);
                UserDto returningUser = CreateUserObject(user);
                ServiceResponse<UserDto>.SuccessResponse(returningUser);
            }

            return new ServiceResponse<UserDto?> { Error = "Please make a stronger password." };
        }

        public async Task<ServiceResponse<UserDto?>> GetCurrentUser()
        {
            User? user = await _userManager.Users
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(x => x.Email == _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email));
            var userDto = CreateUserObject(user!);

            if (userDto is null)
                return new ServiceResponse<UserDto?> { Error = "User not found." };
            else
            {
                await SetRefreshToken(user!);
                return ServiceResponse<UserDto?>.SuccessResponse(userDto);
            }
        }

        public async Task<bool> VerifyFacebookToken(string accessToken)
        {
            string fbVerifyKeys = _config["Facebook:AppId"] + "|" + _config["Facebook:ApiSecret"];
            var verifyTokenResponse = await _httpClient
                .GetAsync($"debug_token?input_token={accessToken}&access_token={fbVerifyKeys}");

            return verifyTokenResponse.IsSuccessStatusCode;
        }

        public async Task<ServiceResponse<UserDto?>> FacebookLogin(string accessToken)
        {
            string fbUrl = $"me?access_token={accessToken}&fields=name,email,picture.width(100).height(100)";
            FacebookDto? fbInfo = await _httpClient.GetFromJsonAsync<FacebookDto>(fbUrl);

            User? user = await _userManager.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(x => x.Email == fbInfo!.Email);

            if (user != null)
                return ServiceResponse<UserDto?>.SuccessResponse(CreateUserObject(user));

            user = new User
            {
                DisplayName = fbInfo!.Name,
                Email = fbInfo.Email,
                UserName = fbInfo.Email,
                Photos = new List<Photo>
                {
                    new Photo
                    {
                        Id = "fb_" + fbInfo.Id,
                        Url = fbInfo.Picture!.Data!.Url,
                        IsMain = true
                    }
                }
            };

            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
                return new ServiceResponse<UserDto?> { Error = "Problem creating user account" };

            await SetRefreshToken(user);
            return ServiceResponse<UserDto?>.SuccessResponse(CreateUserObject(user));
        }

        public async Task<ServiceResponse<UserDto>> RefreshJWT()
        {
            string? refreshToken = _httpContextAccessor.HttpContext!.Request.Cookies["refreshToken"];
            User? user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(u => u.UserName == _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Name));

            RefreshToken? oldToken = user?.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

            if (user is null || oldToken is not null && !oldToken.IsActive) return new ServiceResponse<UserDto> { Error = "No valid Refresh Tokens found." };

            UserDto returning = CreateUserObject(user);
            return ServiceResponse<UserDto>.SuccessResponse(returning);
        }

        private UserDto CreateUserObject(User user)
        {
            return new()
            {
                DisplayName = user.DisplayName,
                Image = user.Photos?.FirstOrDefault(x => x.IsMain)?.Url,
                Token = CreateJWT(user),
                Username = user.UserName
            };
        }

        private static RefreshToken GenerateRefreshToken() 
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return new RefreshToken{Token = Convert.ToBase64String(randomNumber)};
        }

        private async Task SetRefreshToken(User user)
        {
            RefreshToken refreshToken = GenerateRefreshToken();

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            _httpContextAccessor.HttpContext!.Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }
    }
}