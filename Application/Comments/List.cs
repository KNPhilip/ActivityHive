using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments;

public sealed class List
{
    public sealed class Query : IRequest<ServiceResponse<List<CommentDto>>?>
    {
        public Guid ActivityId { get; set; }
    }

    public sealed class Handler(DataContext context, IMapper mapper) 
        : IRequestHandler<Query, ServiceResponse<List<CommentDto>>?>
    {
        private readonly IMapper _mapper = mapper;
        private readonly DataContext _context = context;

        public async Task<ServiceResponse<List<CommentDto>>?> Handle(Query request, CancellationToken cancellationToken)
        {
            List<CommentDto> comments = await _context.Comments
                .Where(x => x.Activity!.Id == request.ActivityId)
                .OrderByDescending(x => x.CreatedAt)
                .ProjectTo<CommentDto>(_mapper.ConfigurationProvider)
                .ToListAsync(CancellationToken.None);

            return ServiceResponse<List<CommentDto>>.SuccessResponse(comments);
        }
    }
}
