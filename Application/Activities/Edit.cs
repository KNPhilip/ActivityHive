using Application.Core;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Edit
    {
        public class Command : IRequest<ServiceResponse<Unit>>
        {
            public Activity? Activity { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(a => a.Activity).SetValidator(new ActivityValidator()!);
            }
        }

        public class Handler(DataContext context, IMapper mapper) 
            : IRequestHandler<Command, ServiceResponse<Unit>>
        {
            private readonly DataContext _context = context;
            private readonly IMapper _mapper = mapper;

            public async Task<ServiceResponse<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                Activity? activity = await _context.Activities
                    .FindAsync([request.Activity!.Id], cancellationToken);
                if (activity is null) 
                {
                    return null!;
                }
                
                _mapper.Map(request.Activity, activity);
                bool result = await _context.SaveChangesAsync(cancellationToken) > 0;
                return result 
                    ? ServiceResponse<Unit>.SuccessResponse(Unit.Value) 
                    : new ServiceResponse<Unit>() { Error = "Failed to update activity." };
            }
        }
    }
}
