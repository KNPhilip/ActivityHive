namespace Domain;

public sealed class Comment
{
    private int id;

    public int Id 
    {
        get => id;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(value));
            id = value;
        }
    }

    public string? Body { get; set; }
    public User? Author { get; set; }
    public Activity? Activity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
