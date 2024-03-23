namespace Application.Comments;

public sealed class CommentDto
{
    private int id;
    private string body = string.Empty;
    private string username = string.Empty;
    private string displayName = string.Empty;
    private string image = string.Empty;

    public int Id 
    {
        get => id;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(value));
            id = value;
        }
    }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string Body 
    {
        get => body;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
            body = value;
        }
    }
    
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
    
    public string Image 
    {
        get => image;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
            image = value;
        }
    }
}
