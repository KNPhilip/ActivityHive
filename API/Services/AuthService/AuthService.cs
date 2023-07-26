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

        public AuthService(UserManager<User> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
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
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

            /* Another option for writing tokens
            
            JwtSecurityToken token = new(
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt; */
        }

        public async Task<ServiceResponse<UserDto?>> Login(LoginDto request)
        {
            User? user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null) return new ServiceResponse<UserDto?> { Error = "Incorrect username or password." };

            bool result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (result)
            {
                UserDto returningUser = new()
                {
                    DisplayName = user.DisplayName,
                    Image = null,
                    Token = CreateJWT(user),
                    Username = user.UserName
                };

                return ServiceResponse<UserDto?>.SuccessResponse(returningUser);
            }
            return new ServiceResponse<UserDto?> { Error = "Incorrect username or password." };
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
                UserDto returningUser = new()
                {
                    DisplayName = user.DisplayName,
                    Image = null,
                    Token = CreateJWT(user),
                    Username = user.UserName
                };

                ServiceResponse<UserDto>.SuccessResponse(returningUser);
            }

            return new ServiceResponse<UserDto?> { Error = "Please make a stronger password." };
        }
    }
}