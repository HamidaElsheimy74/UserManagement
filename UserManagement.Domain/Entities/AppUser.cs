using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Domain.Entities;
public class AppUser : IdentityUser<long>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override long Id { set; get; }
    public DateTime CreatedAt { set; get; } = DateTime.UtcNow;
    public DateTime ModifiedAt { set; get; } = DateTime.UtcNow;
    public DateTime? DeletedAt { set; get; } = null;
    public bool IsDeleted { get; set; } = false;
    public string Email { set; get; }
    public string RefreshToken { set; get; }
    public DateTime? RefreshTokenExpiryTime { set; get; }

    public List<AppUserRoles> UserRoles { set; get; }

}
