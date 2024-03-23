using Domain;

namespace Application.Profiles;

public sealed class Profile
{
    private string username = string.Empty;
    private string displayName = string.Empty;
    private string bio = string.Empty;
    private string image = string.Empty;
    private int followersCount;
    private int followingCount;

    public string Username 
    {
        get => username;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
            username = value;
        }
    }

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

    public string Image 
    {
        get => image;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
            image = value;
        }
    }

    public bool Following { get; set; }

    public int FollowersCount 
    {
        get => followersCount;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(value));
            followersCount = value;
        }
    }

    public int FollowingCount 
    {
        get => followingCount;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(value));
            followingCount = value;
        }
    }

    public ICollection<Photo> Photos { get; set; } = [];
}
