using API.DTOs.Accounts;
using API.Features.Accounts.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class AccountsController(ISender sender): BaseApiController
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        var command = new RegisterUser(registerDto);
        var response = await sender.Send(command);
        
        if (!response.IsSuccess)
        {
            return BadRequest(new { response.Errors });
        }

        return Ok(response.Data);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var command = new Login(loginDto);
        var response = await sender.Send(command);
        
        if (!response.IsSuccess)
        {
            return BadRequest(new { response.Errors });
        }

        return Ok(response.Data);
    }
}