using Application.Activities.Dtos;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _mapper = mapper;
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<ServiceResponse<List<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                List<ActivityDto> activities = await _context.Activities
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, 
                        new {currentUsername = _userAccessor.GetUsername()})
                    .ToListAsync(cancellationToken);

                return ServiceResponse<List<ActivityDto>>.SuccessResponse(activities);
            }
        }
    }
}