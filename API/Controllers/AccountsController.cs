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

        if (response is null)
        {
            return BadRequest("Something went wrong");
        }
        
        if (response.RefreshToken is not null)
        {
            SetRefreshTokenCookie(response.RefreshToken, DateTime.UtcNow.AddDays(7));
        }
        
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var command = new Login(mapper.Map<LoginCommandRequest>(loginDto));
        var response = await sender.Send(command);
        
        if (response.RefreshToken is not null)
        {
            SetRefreshTokenCookie(response.RefreshToken, DateTime.UtcNow.AddDays(7));
        }
        
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<ActionResult<UserDto>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (refreshToken is null)
        {
            return NoContent();
        }

        var commandRequest = new RefreshTokenCommandRequest { RefreshToken = refreshToken };
        var command = new RefreshToken(commandRequest);
        var response = await sender.Send(command);

        if (response is null)
        {
            return Unauthorized("Invalid or expired refresh token");
        }

        if (response.RefreshToken is not null)
        {
            SetRefreshTokenCookie(response.RefreshToken, DateTime.UtcNow.AddDays(7));
        }

        return Ok(response);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        var userId = GetCurrentUserId();
        var command = new Logout(userId);
        var response = await sender.Send(command);

        if (!response)
        {
            return BadRequest("Failed to logout");
        }

        Response.Cookies.Delete("refreshToken");

        return Ok();
    }

    private void SetRefreshTokenCookie(string refreshToken, DateTime expires)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expires,
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}