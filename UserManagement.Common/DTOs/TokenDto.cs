namespace UserManagement.Common.DTOs;
public class TokenDto
{
    public TokenDto()
    {
    }
    public TokenDto(string email, List<string> roles)
    {
        Email = email;
        Roles = roles;
    }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
}
