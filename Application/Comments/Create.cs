using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments;

public sealed class Create
{
    public sealed class Command : IRequest<ServiceResponse<CommentDto>?>
    {
        public string? Body { get; set; }
        public Guid ActivityId { get; set; }
    }

    public sealed class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Body).NotEmpty();
        }
    }

    public sealed class Handler(DataContext context, IMapper mapper, 
        IUserAccessor userAccessor) : IRequestHandler<Command, ServiceResponse<CommentDto>?>
    {
        private readonly DataContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IUserAccessor _userAccessor = userAccessor;

        public async Task<ServiceResponse<CommentDto>?> Handle(Command request, CancellationToken cancellationToken)
        {
            Activity? activity = await _context.Activities.FindAsync(request.ActivityId, CancellationToken.None);

            if (activity is null) 
            {
                return null;
            } 

            User? user = await _context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(p => p.UserName == _userAccessor.GetUsername(), CancellationToken.None);

            Comment comment = new()
            {
                Author = user,
                Activity = activity,
                Body = request.Body
            };

            activity.Comments.Add(comment);

            bool success = await _context.SaveChangesAsync(CancellationToken.None) > 0;
            return success 
                ? ServiceResponse<CommentDto>.SuccessResponse(_mapper.Map<CommentDto>(comment))
                : new ServiceResponse<CommentDto> { Error = "Failed to add comment" };
        }
    }
}
