namespace UserManagement.Application.DTOs;
public class RoleDto
{
    public long roleId { set; get; }
    public string roleName { set; get; }
    public string description { set; get; }
    public DateTime createdAt { set; get; }
    public DateTime updatedAt { set; get; }
}
