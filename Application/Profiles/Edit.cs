using Application.Core;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class Edit
    {
        public class Command : IRequest<ServiceResponse<Unit>?>
        {
            public string? DisplayName { get; set; }
            public string? Bio { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.DisplayName).NotEmpty();
            }
        }

        public class Handler(DataContext context, IUserAccessor userAccessor) 
            : IRequestHandler<Command, ServiceResponse<Unit>?>
        {
            private readonly DataContext _context = context;
            private readonly IUserAccessor _userAccessor = userAccessor;

            public async Task<ServiceResponse<Unit>?> Handle(Command request, CancellationToken cancellationToken)
            {
                User? user = await _context.Users.FirstOrDefaultAsync(
                    x => x.UserName == _userAccessor.GetUsername(), CancellationToken.None);
                if (user is null)
                {
                    return null;
                } 

                user.Bio = request.Bio ?? user.Bio;
                user.DisplayName = request.DisplayName ?? user.DisplayName;

                _context.Entry(user).State = EntityState.Modified;

                bool success = await _context.SaveChangesAsync(CancellationToken.None) > 0;

                return success ? ServiceResponse<Unit>.SuccessResponse(Unit.Value)
                    : new ServiceResponse<Unit> { Error = "Problem updating profile" };
            }
        }
    }
}
