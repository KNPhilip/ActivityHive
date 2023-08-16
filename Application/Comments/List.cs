using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class List
    {
        public class Query : IRequest<ServiceResponse<List<CommentDto>>?>
        {
            public Guid ActivityId { get; set; }
        }

        public class Handler : IRequestHandler<Query, ServiceResponse<List<CommentDto>>?>
        {
            private readonly IMapper _mapper;
            private readonly DataContext _context;
            public Handler(DataContext context, IMapper mapper)
            {
            _context = context;
            _mapper = mapper;
                
            }


            public async Task<ServiceResponse<List<CommentDto>>?> Handle(Query request, CancellationToken cancellationToken)
            {
                List<CommentDto> comments = await _context.Comments
                    .Where(x => x.Activity!.Id == request.ActivityId)
                    .OrderByDescending(x => x.CreatedAt)
                    .ProjectTo<CommentDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return ServiceResponse<List<CommentDto>>.SuccessResponse(comments);
            }
        }
    }
}