using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;


namespace UserManagement.Infrastructure.Data;
public class AppDbContext : IdentityDbContext<AppUser, AppRole, long, IdentityUserClaim<long>, AppUserRoles,
    IdentityUserLogin<long>, IdentityRoleClaim<long>, IdentityUserToken<long>>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<AppUser> AspNetUsers { get; set; }
    public DbSet<AppRole> AspNetRoles { get; set; }
    public DbSet<AppUserRoles> AspNetUserRoles { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<AppUser>(entity =>
        {
            entity.ToTable("AspNetUsers");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.UserName).IsRequired().HasMaxLength(256);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
        });
        builder.Entity<AppRole>(entity =>
        {
            entity.ToTable("AspNetRoles");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Name).IsRequired().HasMaxLength(256);
            entity.Property(r => r.Description).HasMaxLength(512);
        });

        builder.Entity<AppUserRoles>(entity =>
        {
            entity.ToTable("AspNetUserRoles");
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });
            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);
            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
        });
    }
}
