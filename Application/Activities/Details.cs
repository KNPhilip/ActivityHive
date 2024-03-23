using Application.Activities.Dtos;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities;

public sealed class Details
{
    public sealed class Query : IRequest<ServiceResponse<ActivityDto>> 
    {
        public Guid Id { get; set; }
    }

    public sealed class Handler(DataContext context, 
        IMapper mapper, IUserAccessor userAccessor) 
        : IRequestHandler<Query, ServiceResponse<ActivityDto>>
    {
        private readonly DataContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IUserAccessor _userAccessor = userAccessor;

        public async Task<ServiceResponse<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            ActivityDto? activity = await _context.Activities
                .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, 
                    new {currentUsername = _userAccessor.GetUsername()})
                .FirstOrDefaultAsync(x => x.Id == request.Id, CancellationToken.None);

            return ServiceResponse<ActivityDto>.SuccessResponse(activity!);
        }
    }
}
