using System.Security.Claims;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security;

public sealed class IsHostRequirement : IAuthorizationRequirement
{
}

public sealed class IsHostRequirementHandler(DataContext dbContext, 
    IHttpContextAccessor httpContextAccessor) 
    : AuthorizationHandler<IsHostRequirement>
{
    private readonly DataContext _dbContext = dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, IsHostRequirement requirement)
    {
        string? userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) 
        {
            return Task.CompletedTask;
        }

        Guid activityId = Guid.Parse(_httpContextAccessor.HttpContext?.Request.RouteValues
            .SingleOrDefault(x => x.Key == "id").Value?.ToString()!);

        ActivityAttendee? attendee = _dbContext.ActivityAttendees
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.UserId == userId && x.ActivityId == activityId)
            .Result;

        if (attendee is null) 
        {
            return Task.CompletedTask;
        }

        if (attendee.IsHost) 
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
