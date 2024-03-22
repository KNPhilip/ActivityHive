using Application.Core;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities;

public sealed class Edit
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

    public sealed class Handler(DataContext context, IMapper mapper) 
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
