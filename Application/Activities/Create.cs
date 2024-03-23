using Application.Core;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities;

public sealed class Create
{
    public sealed class Command : IRequest<ServiceResponse<Unit>>
    {
        public Activity? Activity { get; set; }
    }

    public sealed class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(a => a.Activity).SetValidator(new ActivityValidator()!);
        }
    }

    public sealed class Handler(DataContext context, IUserAccessor userAccessor) 
        : IRequestHandler<Command, ServiceResponse<Unit>>
    {
        private readonly DataContext _context = context;
        private readonly IUserAccessor _userAccessor = userAccessor;

        public async Task<ServiceResponse<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => 
                u.UserName == _userAccessor.GetUsername(), CancellationToken.None);

            ActivityAttendee attendee = new() 
            {
                User = user,
                Activity = request.Activity,
                IsHost = true
            };

            request.Activity!.Attendees.Add(attendee);

            _context.Activities.Add(request.Activity);

            bool result = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (!result) 
            {
                return new ServiceResponse<Unit>() { Error = "Failed to create activity." };
            }

            return ServiceResponse<Unit>.SuccessResponse(Unit.Value);
        }
    }
}
