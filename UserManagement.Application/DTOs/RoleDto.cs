namespace UserManagement.Application.DTOs;
public class RoleDto
{
    public long RoleId { set; get; }
    public string RoleName { set; get; }
    public string Description { set; get; }
    public DateTime CreatedAt { set; get; }
    public DateTime UpdatedAt { set; get; }
}
