using API.Entities;
using API.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class SeedData
{
    public static async Task InitialiseAdmin(UserManager<User> userManager, AdminSettings adminSettings)
    {
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
        
        await userManager.AddToRoleAsync(user, "Admin");
    }
}