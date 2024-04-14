using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(DataContext context) : ControllerBase
{

    
    
    [HttpGet]
    public ActionResult<IEnumerable<User>> GetUsers()
    {
        var users = context.Users.ToList();

        return users;
    }

    [HttpGet("{id:guid}")]
    public ActionResult<User> GetUser(Guid id)
    { 
        return context.Users.Find(id);
    }
}