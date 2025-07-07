namespace UserManagement.Application.DTOs;
public class UserDto
{
    public long UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<UserRoleDto> UserRoles { get; set; }
}
