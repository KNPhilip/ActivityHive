using API.Dtos;

namespace API.Services.AuthService
{
    public interface IAuthService
    {
        string CreateJWT(User user);
        Task<UserDto?> Login(LoginDto request);
    }
}