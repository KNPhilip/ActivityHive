using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Delete
    {
        public class Command : IRequest<ServiceResponse<Unit>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, ServiceResponse<Unit>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<ServiceResponse<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                Activity? activity = await _context.Activities.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                if(activity is null) return null!;

                _context.Remove(activity);

                bool result = await _context.SaveChangesAsync(cancellationToken) > 0;
                return result ? ServiceResponse<Unit>.SuccessResponse(Unit.Value) : new ServiceResponse<Unit>() { Error = "Failed to delete the activity." };
            }
        }
    }
}