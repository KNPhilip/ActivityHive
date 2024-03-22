using Microsoft.AspNetCore.Identity;

namespace Domain;

public sealed class User : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public ICollection<ActivityAttendee> Activities { get; set; } = [];
    public ICollection<Photo> Photos { get; set; } = [];
    public ICollection<UserFollowing> Followings { get; set; } = [];
    public ICollection<UserFollowing> Followers { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
