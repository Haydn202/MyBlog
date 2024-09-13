using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs.Accounts;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountsController(DataContext context, ITokenService tokenService): BaseApiController
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
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

        return new UserDto
        {
            Name = user.UserName,
            Token = tokenService.CreateToken(user)
        };
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => 
            u.UserName == loginDto.UserName.ToLower());

        if (user == null)
        {
            return Unauthorized("Incorrect username or password.");
        }

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        if (computedHash.Where((t, i) => t != user.PasswordHash[i]).Any())
        {
            return Unauthorized("Incorrect username or password.");
        }

        return new UserDto
        {
            Name = user.UserName,
            Token = tokenService.CreateToken(user)
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync(u => u.UserName == username.ToLower());
    }
}