using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.DTOs.Accounts;
using API.Entities;
using API.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Accounts.Commands;

public class RegisterUser(RegisterUserCommandRequest request) : IRequest<UserDto?>
{
    public RegisterUserCommandRequest Request { get; set; } = request;

    private sealed class RegisterUserHandler(
        UserManager<User> userManager,
        ITokenService tokenService,
        ILogger<RegisterUserHandler> logger)
        : IRequestHandler<RegisterUser, UserDto?>
    {
        public async Task<UserDto?> Handle(RegisterUser request, CancellationToken cancellationToken)
        {
            using var hmac = new HMACSHA512();

            var user = new User
            {
                UserName = request.Request.UserName.ToLower(),
                Email = request.Request.Email
            };

            var result = await userManager.CreateAsync(user, request.Request.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    //TODO figure out a way to return these better
                    logger.LogError(error.Description);
                }
                
                return null;
            }

            // Generate tokens
            var accessToken = await tokenService.CreateToken(user);
            var refreshToken = await tokenService.CreateRefreshToken();
            
            // Update user's refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpires = DateTime.UtcNow.AddDays(7);
            await userManager.UpdateAsync(user);

            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Token = accessToken,
                Email = user.Email,
                RefreshToken = refreshToken
            };
            
            return userDto;
        }
    }
}

public class RegistrationValidator : AbstractValidator<RegisterUser>
{
    private readonly UserManager<User> _userManager;

    public RegistrationValidator(UserManager<User> userManager)
    {
        _userManager = userManager;

        RuleFor(u => u.Request.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .MustAsync(async (username, cancellationToken) => 
                !await UserExists(username)).WithMessage("Username is taken");
        
        RuleFor(u => u.Request.Email)
            .NotEmpty().WithMessage("Email is required.")
            .MustAsync(async (email, cancellationToken) =>
                !await EmailExits(email)).WithMessage("Email is taken");

        RuleFor(u => u.Request.Password)
            .NotEmpty().WithMessage("Password is required.");
    }

    private async Task<bool> EmailExits(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        
        return user != null;
    }

    private async Task<bool> UserExists(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        
        return user != null;
    }
}
