using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Data.SeedData;
public class SeedAppData
{
    public static async Task SeedAsync(AppDbContext context, ILoggerFactory loggerFactory, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        try
        {
            if (!userManager.Users.Any())
            {
                var creatAt = DateTime.UtcNow;
                AppUser createdUser = new AppUser();
                IdentityResult createdRole = new IdentityResult();
                var users = new List<(AppUser User, AppRole role, string Password)>
                {
                    (new AppUser {
                       UserName = "HamidaAdmin",
                        Email = "admin@gmail.com", CreatedAt = creatAt,
                    },  new AppRole{Name ="Admin",Description = "Admin Role", CreatedAt = creatAt },"Admin@1234"),
                    (new AppUser
                    {
                        UserName = "HamidaUser",
                        Email = "User@gmail.com", CreatedAt = creatAt },
                        new AppRole{Name = "User", Description = "User Role ", CreatedAt = creatAt  }, "User@1234")
                };

                foreach (var (user, role, password) in users)
                {

                    var identityRole = await roleManager.CreateAsync(role);


                    if (identityRole.Succeeded)
                    {
                        var identityUser = await userManager.CreateAsync(user, password);

                        if (identityUser.Succeeded)
                        {
                            await context.AspNetUserRoles.AddAsync(new AppUserRoles
                            {
                                UserId = user.Id,
                                RoleId = role.Id,
                                CreatedAt = creatAt,
                            });

                            await context.SaveChangesAsync();
                        }
                    }

                }
            }
        }
        catch (Exception ex)
        {
            var logger = loggerFactory.CreateLogger<SeedAppData>();
            logger.LogError($"An error occurred during seeding DB: {ex.Message}");
        }
    }
}
