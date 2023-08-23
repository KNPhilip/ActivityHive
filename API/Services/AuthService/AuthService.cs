using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        public AuthService(UserManager<User> userManager, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }

        public string CreateJWT(User user)
        {
            List<Claim> claims = new() 
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["AppSettings:Token"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
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
                return ServiceResponse<UserDto?>.SuccessResponse(userDto);
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
    }
}