using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos;

public sealed class SetMain
{
    public sealed class Command : IRequest<ServiceResponse<Unit>?>
    {
        public string? Id { get; set; }
    }

    public sealed class Handler(DataContext context, IUserAccessor userAccessor) 
        : IRequestHandler<Command, ServiceResponse<Unit>?>
    {
        private readonly IUserAccessor _userAccessor = userAccessor;
        private readonly DataContext _context = context;

        public async Task<ServiceResponse<Unit>?> Handle(
            Command request, CancellationToken cancellationToken)
        {
            User? user = await _context.Users
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(u => u.UserName == _userAccessor
                    .GetUsername(), CancellationToken.None);

            if (user is null) 
            {
                return null;
            } 

            Photo? photo = user.Photos.FirstOrDefault(x => x.Id == request.Id);

            if (photo is null) 
            {
                return null;
            } 

            Photo? currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

            if (currentMain is not null) 
            {
                currentMain.IsMain = false;
            }
            photo.IsMain = true;

            bool success = await _context.SaveChangesAsync(CancellationToken.None) > 0;
            if (success) 
            {
                return ServiceResponse<Unit>.SuccessResponse(Unit.Value);
            }
            return new ServiceResponse<Unit> { Error = "Problem setting main photo" };
        }
    }
}
