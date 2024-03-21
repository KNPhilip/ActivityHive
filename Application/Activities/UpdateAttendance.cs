using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class UpdateAttendance
    {
        public class Command : IRequest<ServiceResponse<Unit>?>
        {
            public Guid Id { get; set; }
        }

        public class Handler(DataContext context, IUserAccessor userAccessor) 
            : IRequestHandler<Command, ServiceResponse<Unit>?>
        {
            private readonly IUserAccessor _userAccessor = userAccessor;
            private readonly DataContext _context = context;

            public async Task<ServiceResponse<Unit>?> Handle(Command request, CancellationToken cancellationToken)
            {
                Activity? activity = await _context.Activities
                    .Include(a => a.Attendees!)
                    .ThenInclude(u => u.User)
                    .FirstOrDefaultAsync(x => x.Id == request.Id, CancellationToken.None);
                if (activity is null) 
                {
                    return null;
                } 

                User? user = await _context.Users.FirstOrDefaultAsync(x => 
                    x.UserName == _userAccessor.GetUsername(), CancellationToken.None);
                if (user is null)
                {
                    return null;
                }

                string? hostUsername = activity.Attendees?.FirstOrDefault(x => x.IsHost)?.User!.UserName;

                ActivityAttendee? attendant = activity.Attendees?.FirstOrDefault(x => x.User!.UserName == user.UserName);

                if (attendant is not null && hostUsername == user.UserName) 
                {
                    activity.IsCancelled = !activity.IsCancelled;
                }
                if (attendant is not null && hostUsername != user.UserName) 
                {
                    activity.Attendees!.Remove(attendant);
                }

                if (attendant is null)
                {
                    attendant = new ActivityAttendee
                    {
                        User = user,
                        Activity = activity,
                        IsHost = false
                    };

                    activity.Attendees!.Add(attendant);
                }

                bool successResponse = await _context.SaveChangesAsync(CancellationToken.None) > 0;
                return successResponse 
                    ? ServiceResponse<Unit>.SuccessResponse(Unit.Value) 
                    : new ServiceResponse<Unit> { Error = "Problem updating attendance..." };
            }
        }
    }
}
