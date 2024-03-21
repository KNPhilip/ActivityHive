using System.Security.Claims;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Security
{
    public class UserAccessor(IHttpContextAccessor httpContextAccessor) : IUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public string GetUsername() 
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name)!;
        }
    }
}
