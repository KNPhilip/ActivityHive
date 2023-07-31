using Application.Activities.Dtos;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<ServiceResponse<List<ActivityDto>>> 
        {

        }

        public class Handler : IRequestHandler<Query, ServiceResponse<List<ActivityDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<ServiceResponse<List<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                List<ActivityDto> activities = await _context.Activities
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return ServiceResponse<List<ActivityDto>>.SuccessResponse(activities);
            }
        }
    }
}