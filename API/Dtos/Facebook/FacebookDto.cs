using API.Dtos.Facebook;

namespace API.Dtos
{
    public class FacebookDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public FacebookPictureDto? Picture { get; set; }
    }
}