using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class User : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public ICollection<ActivityAttendee> Activities { get; set; } = new List<ActivityAttendee>();
        public ICollection<Photo> Photos { get; set; } = new List<Photo>();
    }
}