using API.DTOs.Accounts;
using API.Features.Accounts.Commands;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class AccountsController(
    IMapper mapper,
    ISender sender): BaseApiController
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        var command = new RegisterUser(mapper.Map<RegisterUserCommandRequest>(registerDto));
        var response = await sender.Send(command);
        
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var command = new Login(mapper.Map<LoginCommandRequest>(loginDto));
        var response = await sender.Send(command);
        
        return Ok(response);
    }
}