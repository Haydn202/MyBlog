using API.Data;
using API.DTOs.Accounts;
using API.DTOs.User;
using API.Entities;
using API.Features.Users.Command;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class UsersController(
    DataContext context,
    ISender sender) : BaseApiController
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

    [HttpPost("{id:guid}")]
    public async Task<Results<NotFound, Ok<UserDto>>> UpdateUserRole([FromBody] RoleUpdateDto request, [FromRoute] Guid id)
    {
        var command = new UpdateRole(request, id);
        var response = await sender.Send(command);

        if (response is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(response);
    }
}