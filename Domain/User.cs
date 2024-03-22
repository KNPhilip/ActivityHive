using Microsoft.AspNetCore.Identity;

namespace Domain;

public sealed class User : IdentityUser
{
    private string displayName = string.Empty;
    private string bio = string.Empty;

    public string DisplayName 
    {
        get => displayName;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
            displayName = value;
        }
    }

    public string Bio 
    {
        get => bio;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
            bio = value;
        }
    }
    public ICollection<ActivityAttendee> Activities { get; set; } = [];
    public ICollection<Photo> Photos { get; set; } = [];
    public ICollection<UserFollowing> Followings { get; set; } = [];
    public ICollection<UserFollowing> Followers { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
