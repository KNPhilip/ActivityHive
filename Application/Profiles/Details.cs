using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class Details
    {
        public class Query : IRequest<ServiceResponse<Profile>?>
        {
            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Query, ServiceResponse<Profile>?>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<ServiceResponse<Profile>?> Handle(Query request, CancellationToken cancellationToken)
            {
                Profile? profile = await _context.Users
                    .ProjectTo<Profile>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(x => x.Username == request.Username, CancellationToken.None);

                if (profile is null) return null;

                return ServiceResponse<Profile>.SuccessResponse(profile!);
            }
        }
    }
}