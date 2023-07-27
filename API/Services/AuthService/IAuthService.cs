using API.Dtos;
using Application.Core;

namespace API.Services.AuthService
{
    public interface IAuthService
    {
        string CreateJWT(User user);
        Task<ServiceResponse<UserDto?>> Login(LoginDto request);
        Task<ServiceResponse<UserDto?>> Register(RegisterDto request);
        Task<ServiceResponse<UserDto?>> GetCurrentUser();
    }
}