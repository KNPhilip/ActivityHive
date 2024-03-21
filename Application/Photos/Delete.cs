using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Delete
    {
        public class Command : IRequest<ServiceResponse<Unit>?>
        {
            public string? Id { get; set; }
        }

        public class Handler(DataContext context, IPhotoAccessor photoAccessor, 
            IUserAccessor userAccessor) : IRequestHandler<Command, ServiceResponse<Unit>?>
        {
            private readonly IPhotoAccessor _photoAccessor = photoAccessor;
            private readonly IUserAccessor _userAccessor = userAccessor;
            private readonly DataContext _context = context;

            public async Task<ServiceResponse<Unit>?> Handle(Command request, CancellationToken cancellationToken)
            {
                User? user = await _context.Users
                    .Include(u => u.Photos)
                    .FirstOrDefaultAsync(x => x.UserName == _userAccessor
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
                if (photo.IsMain) 
                {
                    return new ServiceResponse<Unit> { Error = "You cannot delete your main photo." };
                }

                string? result = await _photoAccessor.DeletePhoto(photo.Id!);
                if (result is null) 
                {
                    return new ServiceResponse<Unit> { Error = "Problem deleting photo from Cloudinary" };
                }

                user.Photos.Remove(photo);

                bool success = await _context.SaveChangesAsync(CancellationToken.None) > 0;
                if (success) 
                {
                    return ServiceResponse<Unit>.SuccessResponse(Unit.Value);
                }
                return new ServiceResponse<Unit> { Error = "Problem deleting photo from API" };
            }
        }
    }
}
