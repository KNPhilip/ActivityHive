using System.ComponentModel.DataAnnotations;

namespace API.Dtos;

public sealed class RegisterDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [
        Required, 
        //Each set of parantheses/curley brackets within the RegularExpression string represents password rules.
        //Rules are: Contains a number, lowercase & uppercase letter, and is within 4-8 characters long (in that order) 
        RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{4,8}$", 
        ErrorMessage = "Password must be complex")
    ]
    public string Password { get; set; } = string.Empty;
    [Required]
    public string DisplayName { get; set; } = string.Empty;
    [Required]
    public string Username { get; set; } = string.Empty;
}
