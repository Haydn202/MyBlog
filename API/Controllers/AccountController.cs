using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context): BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.Username))
        {
            return BadRequest("Username is Taken");
        }
        
        using var hmac = new HMACSHA512();

        var user = new User
        {
            Id = default,
            UserName = registerDto.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    [HttpPost("login")]
    public async Task<ActionResult<User>> Login(LoginDto loginDto)
    {
        var user = context.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);
        
        
    }

    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync(u => u.UserName == username.ToLower());
    }

    private bool PasswordsMatch(string password, User user)
    {
        using var hmac = new HMACSHA512();

        return user.PasswordHash == hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }
}