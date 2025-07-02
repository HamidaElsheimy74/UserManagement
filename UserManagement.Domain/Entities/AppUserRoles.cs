using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Domain.Entities;
public class AppUserRoles : IdentityUserRole<long>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { set; get; }
    public DateTime CreatedAt { set; get; } = DateTime.UtcNow;
    public DateTime ModifiedAt { set; get; } = DateTime.UtcNow;
    public DateTime? DeletedAt { set; get; } = null;
    public bool IsDeleted { get; set; } = false;
    public long UserId { get; set; }
    public AppUser User { get; set; }
    public long RoleId { get; set; }
    public AppRole Role { get; set; }
}
