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
    public class Details
    {
        public class Query : IRequest<ServiceResponse<ActivityDto>> 
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, ServiceResponse<ActivityDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _mapper = mapper;
                _context = context;
            }

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
}