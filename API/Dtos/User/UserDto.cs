namespace API.Dtos
{
    public class UserDto
    {
        public string DisplayName { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? Image { get; set; }
        public string? Username { get; set; }
    }
}
