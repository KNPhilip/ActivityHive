using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore.Query;
using Persistence;

namespace Application.Activities
{
    public class Details
    {
        public class Query : IRequest<ServiceResponse<Activity>> 
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, ServiceResponse<Activity>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<ServiceResponse<Activity>> Handle(Query request, CancellationToken cancellationToken)
            {
                Activity activity = await _context.Activities.FindAsync(request.Id);

                return ServiceResponse<Activity>.SuccessResponse(activity);
            }
        }
    }
}