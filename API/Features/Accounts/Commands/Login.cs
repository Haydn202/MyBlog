using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.DTOs.Accounts;
using API.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Accounts.Commands;

public class Login(LoginCommandRequest request) : IRequest<UserDto>
{
    public LoginCommandRequest Request { get; set; } = request;
    
    private sealed class LoginHandler(
        DataContext dbContext, 
        ITokenService tokenService) 
        : IRequestHandler<Login, UserDto>
    {
        public async Task<UserDto> Handle(Login request, CancellationToken cancellationToken)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => 
                u.UserName == request.Request.UserName.ToLower());

            if (user == null)
            {
                throw new InvalidOperationException("Incorrect username or password.");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Token = tokenService.CreateToken(user),
                Role = user.Role.ToString()
            };

            return userDto;
        }
    }
}

public class LoginValidator : AbstractValidator<Login>
{
    private readonly DataContext _dbContext;

    public LoginValidator(DataContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(u => u.Request.UserName)
            .NotEmpty().WithMessage("Username is required.");

        RuleFor(u => u.Request.Password)
            .NotEmpty().WithMessage("Password is required.");
        
        RuleFor(u => u.Request)
            .NotEmpty().WithMessage("Password is required.")
            .MustAsync(async (request, cancellationToken) => 
                await PasswordIsValid(request)).WithMessage("Incorrect username or password.");
    }
    
    private async Task<bool> PasswordIsValid(LoginCommandRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName.ToLower());
        
        if (user is null)
        {
            return false;
        }
        
        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));

        if (computedHash.Where((t, i) => t != user.PasswordHash[i]).Any())
        {
            return false;
        }

        return true;
    }
}