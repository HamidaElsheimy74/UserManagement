using System.ComponentModel.DataAnnotations;

namespace UserManagement.Application.DTOs;
public class LoginDto
{
    [Required]
    public string UserName { set; get; }

    [Required]
    [MinLength(8)]
    [MaxLength(20)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character.")]

    public string Password { set; get; }
}
