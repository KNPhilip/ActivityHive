using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles;

public sealed class Details
{
    public sealed class Query : IRequest<ServiceResponse<Profile>?>
    {
        public string? Username { get; set; }
    }

    public sealed class Handler(DataContext context, IMapper mapper, 
        IUserAccessor userAccessor) : IRequestHandler<Query, ServiceResponse<Profile>?>
    {
        private readonly DataContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IUserAccessor _userAccessor = userAccessor;

        public async Task<ServiceResponse<Profile>?> Handle(
            Query request, CancellationToken cancellationToken)
        {
            Profile? profile = await _context.Users
                .ProjectTo<Profile>(_mapper.ConfigurationProvider,
                    new {currentUsername = _userAccessor.GetUsername()})
                .FirstOrDefaultAsync(x => x.Username == request.Username, CancellationToken.None);

            if (profile is null) 
            {
                return null;
            }

            return ServiceResponse<Profile>.SuccessResponse(profile!);
        }
    }
}
