using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers
{
    public class List
    {
        public class Query : IRequest<ServiceResponse<List<Profiles.Profile>>>
        {
            public string? Predicate { get; set; }
            public string? Username { get; set; }
        }

        public class Handler(DataContext context, IMapper mapper, 
            IUserAccessor userAccessor) : IRequestHandler<Query, ServiceResponse<List<Profiles.Profile>>>
        {
            private readonly DataContext _context = context;
            private readonly IMapper _mapper = mapper;
            private readonly IUserAccessor _userAccessor = userAccessor;

            public async Task<ServiceResponse<List<Profiles.Profile>>> Handle(Query request, CancellationToken cancellationToken)
            {
                List<Profiles.Profile>? profiles = [];

                switch (request.Predicate){
                    case "followers":
                        profiles = await _context.UserFollowings
                            .Where(x => x.Target!.UserName == request.Username)
                            .Select(u => u.Observer)
                            .ProjectTo<Profiles.Profile>(_mapper.ConfigurationProvider,
                                new {currentUsername = _userAccessor.GetUsername()})
                            .ToListAsync(CancellationToken.None);
                        break;
                    case "following":
                        profiles = await _context.UserFollowings
                            .Where(x => x.Observer!.UserName == request.Username)
                            .Select(u => u.Observer)
                            .ProjectTo<Profiles.Profile>(_mapper.ConfigurationProvider,
                                new {currentUsername = _userAccessor.GetUsername()})
                            .ToListAsync(CancellationToken.None);
                        break;
                }

                return ServiceResponse<List<Profiles.Profile>>
                    .SuccessResponse(profiles);
            }
        }
    }
}
