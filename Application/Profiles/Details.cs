using Application.Core;
using Application.Interfaces;
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
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _context = context;
                _mapper = mapper;
                _userAccessor = userAccessor;
            }

            public async Task<ServiceResponse<Profile>?> Handle(Query request, CancellationToken cancellationToken)
            {
                Profile? profile = await _context.Users
                    .ProjectTo<Profile>(_mapper.ConfigurationProvider,
                        new {currentUsername = _userAccessor.GetUsername()})
                    .FirstOrDefaultAsync(x => x.Username == request.Username, CancellationToken.None);

                if (profile is null) return null;

                return ServiceResponse<Profile>.SuccessResponse(profile!);
            }
        }
    }
}