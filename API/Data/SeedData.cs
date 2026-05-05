using API.Entities;
using API.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class SeedData
{
    public static async Task InitialiseAdmin(
        RoleManager<IdentityRole> roleManager,
        UserManager<User> userManager,
        AdminSettings adminSettings)
    {
        foreach (var roleName in new[] { nameof(Role.Admin), nameof(Role.Contributor) })
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var created = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!created.Succeeded)
            {
                throw new Exception(created.Errors.First().Description);
            }
        }

        if (await userManager.Users.AnyAsync())
        {
            return;
        }

        var user = new User
        {
            UserName = adminSettings.UserName,
            Email = adminSettings.Email,
            EmailConfirmed = true,
        };
        var res = await userManager.CreateAsync(user, adminSettings.Password);

        if (!res.Succeeded)
        {
            throw new Exception(res.Errors.First().Description);
        }

        await userManager.AddToRoleAsync(user, nameof(Role.Admin));
    }
}
