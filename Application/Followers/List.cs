using Application.Core;
using Application.Interfaces;
using Application.Profiles;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using MediatR.Registration;
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

        public class Handler : IRequestHandler<Query, ServiceResponse<List<Profiles.Profile>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<ServiceResponse<List<Profiles.Profile>>> Handle(Query request, CancellationToken cancellationToken)
            {
                List<Profiles.Profile>? profiles = new List<Profiles.Profile>();

                switch (request.Predicate){
                    case "followers":
                        profiles = await _context.UserFollowings
                            .Where(x => x.Target!.UserName == request.Username)
                            .Select(u => u.Observer)
                            .ProjectTo<Profiles.Profile>(_mapper.ConfigurationProvider)
                            .ToListAsync(CancellationToken.None);
                        break;
                    case "following":
                        profiles = await _context.UserFollowings
                            .Where(x => x.Observer!.UserName == request.Username)
                            .Select(u => u.Observer)
                            .ProjectTo<Profiles.Profile>(_mapper.ConfigurationProvider)
                            .ToListAsync(CancellationToken.None);
                        break;
                }

                return ServiceResponse<List<Profiles.Profile>>
                    .SuccessResponse(profiles);
            }
        }
    }
}