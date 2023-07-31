using Application.Activities.Dtos;
using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Persistence;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<ServiceResponse<List<ActivityDto>>> 
        {

        }

        public class Handler : IRequestHandler<Query, ServiceResponse<List<ActivityDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<ServiceResponse<List<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                List<Activity> activities = await _context.Activities
                    .Include(a => a.Attendees)
                    .ThenInclude(u => u.User)
                    .ToListAsync(cancellationToken);

                List<ActivityDto> response = _mapper.Map<List<ActivityDto>>(activities);

                return ServiceResponse<List<ActivityDto>>.SuccessResponse(response);
            }
        }
    }
}