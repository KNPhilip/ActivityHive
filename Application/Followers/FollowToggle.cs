using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers;

public sealed class FollowToggle
{
    public sealed class Command : IRequest<ServiceResponse<Unit>?>
    {
        public string? TargetUsername { get; set; }
    }

    public sealed class Handler(DataContext context, IUserAccessor userAccessor) 
        : IRequestHandler<Command, ServiceResponse<Unit>?>
    {
        private readonly DataContext _context = context;
        private readonly IUserAccessor _userAccessor = userAccessor;

        public async Task<ServiceResponse<Unit>?> Handle(
            Command request, CancellationToken cancellationToken)
        {
            User? observer = await _context.Users
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername(), CancellationToken.None);

            User? target = await _context.Users
                .FirstOrDefaultAsync(x => x.UserName == request.TargetUsername, CancellationToken.None);

            if (target is null) 
            {
                return null;
            }

            UserFollowing? following = await _context.UserFollowings
                .FindAsync(observer!.Id, target.Id);

            if (following is null)
            {
                following = new UserFollowing
                {
                    Observer = observer,
                    Target = target
                };

                _context.UserFollowings.Add(following);
            }
            else 
            {
                _context.UserFollowings.Remove(following);
            }

            bool success = await _context
                .SaveChangesAsync(CancellationToken.None) > 0;

            return success
                ? ServiceResponse<Unit>.SuccessResponse(Unit.Value) 
                : new ServiceResponse<Unit> { Error = "Failed to update following" };
        }
    }
}
