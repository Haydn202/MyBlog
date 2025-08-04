using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.DTOs.Accounts;
using API.Entities;
using API.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Accounts.Commands;

public class RegisterUser(RegisterUserCommandRequest request) : IRequest<UserDto>
{
    public RegisterUserCommandRequest Request { get; set; } = request;

    private sealed class RegisterUserHandler(
        DataContext dbContext,
        ITokenService tokenService)
        : IRequestHandler<RegisterUser, UserDto>
    {
        public async Task<UserDto> Handle(RegisterUser request, CancellationToken cancellationToken)
        {
            using var hmac = new HMACSHA512();

            var user = new User
            {
                Id = default,
                UserName = request.Request.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Request.Password)),
                PasswordSalt = hmac.Key,
                Role = Role.None
            };

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync(cancellationToken);

            var userDto = new UserDto
            {
                UserName = user.UserName,
                Token = tokenService.CreateToken(user)
            };
            
            return userDto;
        }
    }
}

public class RegistrationValidator : AbstractValidator<RegisterUser>
{
    private readonly DataContext _dbContext;

    public RegistrationValidator(DataContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(u => u.Request.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MustAsync(async (username, cancellationToken) => 
                !await UserExists(username)).WithMessage("Username is taken");

        RuleFor(u => u.Request.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }

    private async Task<bool> UserExists(string username)
    {
        return await _dbContext.Users.AnyAsync(u => u.UserName == username.ToLower());
    }
}
