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

public class Login(LoginCommandRequest request) : IRequest<UserDto>
{
    public LoginCommandRequest Request { get; } = request;
    
    private sealed class LoginHandler(
        UserManager<User> userManager, 
        ITokenService tokenService) 
        : IRequestHandler<Login, UserDto>
    {
        public async Task<UserDto> Handle(Login request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Request.Email);

            if (user == null)
            {
                throw new InvalidOperationException("Incorrect email or password.");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Token = await tokenService.CreateToken(user),
                Email = user.Email,
            };

            return userDto;
        }
    }
}

public class LoginValidator : AbstractValidator<Login>
{
    private readonly UserManager<User> _userManager;

    public LoginValidator(UserManager<User> userManager)
    {
        _userManager = userManager;

        RuleFor(u => u.Request.Email)
            .NotEmpty().WithMessage("Email is required.");

        RuleFor(u => u.Request.Password)
            .NotEmpty().WithMessage("Password is required.");
        
        RuleFor(u => u.Request)
            .NotEmpty().WithMessage("Password is required.")
            .MustAsync(async (request, cancellationToken) => 
                await PasswordIsValid(request)).WithMessage("Incorrect email or password.");
    }
    
    private async Task<bool> PasswordIsValid(LoginCommandRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        
        if (user is null)
        {
            return false;
        }
        
        var passwordIsValid = await _userManager.CheckPasswordAsync(user, request.Password);
        
        return passwordIsValid;
    }
}