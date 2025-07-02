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
                var users = new List<(AppUser User, string Role, string description, string Password)>
                {
                    (new AppUser {
                       UserName = "HamidaAdmin",
                        Email = "admin@gmail.com"
                    }, "Admin", "Admin Role","Admin@1234"),
                    (new AppUser
                    {
                        UserName = "HamidaUser",
                        Email = "User@gmail.com" },
                        "User", "User Role ", "User@1234")
                };

                foreach (var (user, role, description, password) in users)
                {

                    var createUser = await userManager.CreateAsync(user, password);


                    if (createUser.Succeeded)
                    {
                        var identityRole = new AppRole { Name = role, Description = description };
                        var createdRole = await roleManager.CreateAsync(identityRole);

                        if (createdRole.Succeeded)
                        {
                            await userManager.AddToRoleAsync(user, role);
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
