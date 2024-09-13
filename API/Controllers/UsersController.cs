using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class UsersController(DataContext context) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await context.Users.ToListAsync();

        return users;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<User>> GetUser(Guid id)
    { 
        return await context.Users.FindAsync(id);
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser()
    {
        var user = new User
        {
            Id = new Guid(),
            UserName = "Sally",
            PasswordHash = new byte[]
            {
            },
            PasswordSalt = new byte[]
            {
            }
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return user;
    }
}