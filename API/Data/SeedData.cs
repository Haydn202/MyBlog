using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class SeedData
{
    public static async Task InitialiseAdmin(UserManager<User> userManager)
    {
        if (await userManager.Users.AnyAsync())
        {
            return;
        }

        var user = new User
        {
            UserName = "admin",
            Email = "Admin@email.com",
            EmailConfirmed = true,
        };
        var res = await userManager.CreateAsync(user, "admin");

        if (!res.Succeeded)
        {
            throw new Exception(res.Errors.First().Description);
        }
        
        await userManager.AddToRoleAsync(user, "Admin");
    }
}