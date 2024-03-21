using Application.Activities.Dtos;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<ServiceResponse<PagedList<ActivityDto>>> 
        {
            public ActivityParams? Params { get; set; }
        }

        public class Handler(DataContext context, 
            IMapper mapper, IUserAccessor userAccessor) 
            : IRequestHandler<Query, ServiceResponse<PagedList<ActivityDto>>>
        {
            private readonly DataContext _context = context;
            private readonly IMapper _mapper = mapper;
            private readonly IUserAccessor _userAccessor = userAccessor;

            public async Task<ServiceResponse<PagedList<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                IQueryable<ActivityDto> query = _context.Activities
                    .Where(d => d.Date >= request.Params!.StartDate)
                    .OrderBy(a => a.Date)
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, 
                        new {currentUsername = _userAccessor.GetUsername()})
                    .AsQueryable();

                if (request.Params!.IsGoing && !request.Params.IsHost) 
                {
                    query = query.Where(x => x.Attendees.Any(a => a.Username == _userAccessor.GetUsername()));
                }

                if (request.Params.IsHost && !request.Params.IsGoing) 
                {
                    query = query.Where(x => x.HostUsername == _userAccessor.GetUsername());
                }

                return ServiceResponse<PagedList<ActivityDto>>.SuccessResponse(
                    await PagedList<ActivityDto>
                        .CreateAsync(query, request.Params!.PageNumber, request.Params.PageSize)
                );
            }
        }
    }
}
