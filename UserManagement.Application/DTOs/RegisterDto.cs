using System.ComponentModel.DataAnnotations;

namespace UserManagement.Application.DTOs;
public class RegisterDto
{
    [Required]
    [EmailAddress]
    [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }
    [Required]
    [MinLength(8)]
    [MaxLength(20)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character.")]
    public string Password { get; set; }

    [Required]
    public string UserName { get; set; }
}
