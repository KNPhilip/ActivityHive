using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles;

public sealed class ListActivities
{
    public sealed class Query : IRequest<ServiceResponse<List<UserActivityDto>>?>
    {
        public string? Username { get; set; }
        public string? Predicate { get; set; }
    }

    public sealed class Handler(DataContext context, IMapper mapper)
        : IRequestHandler<Query, ServiceResponse<List<UserActivityDto>>?>
    {
        private readonly IMapper _mapper = mapper;
        private readonly DataContext _context = context;

        public async Task<ServiceResponse<List<UserActivityDto>>?> Handle(
            Query request, CancellationToken cancellationToken)
        {
            IQueryable<UserActivityDto> query = _context.ActivityAttendees
                .Where(u => u.User!.UserName == request.Username)
                .OrderBy(a => a.Activity!.Date)
                .ProjectTo<UserActivityDto>(_mapper.ConfigurationProvider)
                .AsQueryable();

            query = request.Predicate switch
            {
                "past" => query.Where(a => a.Date <= DateTime.UtcNow),
                "hosting" => query.Where(a => a.HostUsername == request.Username),
                _ => query.Where(a => a.Date >= DateTime.UtcNow)
            };

            List<UserActivityDto> activities = await query.ToListAsync(CancellationToken.None);

            return ServiceResponse<List<UserActivityDto>>.SuccessResponse(activities);
        }
    }
}
