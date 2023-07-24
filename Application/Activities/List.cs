using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Persistence;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<ServiceResponse<List<Activity>>> 
        {

        }

        public class Handler : IRequestHandler<Query, ServiceResponse<List<Activity>>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<ServiceResponse<List<Activity>>> Handle(Query request, CancellationToken cancellationToken)
            {
                return ServiceResponse<List<Activity>>.SuccessResponse(await _context.Activities.ToListAsync(cancellationToken));
            }
        }
    }
}