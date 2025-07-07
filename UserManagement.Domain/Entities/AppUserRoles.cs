using Microsoft.AspNetCore.Identity;

namespace UserManagement.Domain.Entities;
public class AppUserRoles : IdentityUserRole<long>
{
    public DateTime CreatedAt { set; get; } = DateTime.UtcNow;
    public DateTime ModifiedAt { set; get; } = DateTime.UtcNow;
    public DateTime? DeletedAt { set; get; } = null;
    public bool IsDeleted { get; set; } = false;
    public AppUser User { get; set; }
    public AppRole Role { get; set; }
}
