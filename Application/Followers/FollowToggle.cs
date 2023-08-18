using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers
{
    public class FollowToggle
    {
        public class Command : IRequest<ServiceResponse<Unit>?>
        {
            public string? TargetUsername { get; set; }
        }

        public class Handler : IRequestHandler<Command, ServiceResponse<Unit>?>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<ServiceResponse<Unit>?> Handle(Command request, CancellationToken cancellationToken)
            {
                User? observer = await _context.Users
                    .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername(), CancellationToken.None);

                User? target = await _context.Users
                    .FirstOrDefaultAsync(x => x.UserName == request.TargetUsername, CancellationToken.None);

                if (target is null) return null;

                UserFollowing? following = await _context.UserFollowings
                    .FindAsync(observer!.Id, target.Id);

                if (following == null)
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
}