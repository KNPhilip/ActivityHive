namespace API.Dtos;

public sealed class UserDto
{
    public string DisplayName { get; set; } = string.Empty;
    public string? Token { get; set; }
    public string? Image { get; set; }
    public string? Username { get; set; }
}
