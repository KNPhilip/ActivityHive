using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Add
    {
        public class Command : IRequest<ServiceResponse<Photo>?>
        {
            public IFormFile? File { get; set; }
        }

        public class Handler : IRequestHandler<Command, ServiceResponse<Photo>?>
        {
            private readonly DataContext _context;
            private readonly IPhotoAccessor _photoAccessor;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IPhotoAccessor photoAccessor, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _photoAccessor = photoAccessor;
                _context = context;
            }

            public async Task<ServiceResponse<Photo>?> Handle(Command request, CancellationToken cancellationToken)
            {
                User? user = await _context.Users
                    .Include(p => p.Photos)
                    .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername(), CancellationToken.None);

                if (user is null) return null;

                var photoUploadResult = await _photoAccessor.AddPhoto(request.File!);

                Photo photo = new()
                {
                    Url = photoUploadResult!.Url,
                    Id = photoUploadResult.PublicId
                };

                if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;

                user.Photos.Add(photo);

                bool result = await _context.SaveChangesAsync(CancellationToken.None) > 0;

                if (result) return ServiceResponse<Photo>.SuccessResponse(photo);

                return new ServiceResponse<Photo> { Error = "Problem adding photo" };
            }
        }
    }
}