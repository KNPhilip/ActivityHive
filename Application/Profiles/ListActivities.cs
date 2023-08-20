using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Persistence;

namespace Application.Profiles
{
    public class ListActivities
    {
        public class Query : IRequest<ServiceResponse<List<UserActivityDto>>?>
        {
            public string? Username { get; set; }
            public string? Predicate { get; set; }
        }

        public class Handler : IRequestHandler<Query, ServiceResponse<List<UserActivityDto>>?>
        {
            private readonly IMapper _mapper;
            private readonly DataContext _context;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<ServiceResponse<List<UserActivityDto>>?> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.ActivityAttendees
                    .Where(u => u.User.UserName == request.Username)
                    .OrderBy(a => a.Activity.Date)
                    .ProjectTo<UserActivityDto>(_mapper.ConfigurationProvider)
                    .AsQueryable();

                query = request.Predicate switch
                {
                    "past" => query.Where(a => a.Date <= DateTime.Now),
                    "hosting" => query.Where(a => a.HostUsername == request.Username),
                    _ => query.Where(a => a.Date >= DateTime.Now)
                };

                List<UserActivityDto> activities = await query.ToListAsync(CancellationToken.None);

                return ServiceResponse<List<UserActivityDto>>.SuccessResponse(activities);
            }
        }
    }
}