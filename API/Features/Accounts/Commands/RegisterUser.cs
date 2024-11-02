using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs.Accounts;
using API.Entities;
using API.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Accounts.Commands;

public class RegisterUser(RegisterDto request) : IRequest<UserDto>
{
    public RegisterDto Request { get; } = request;

    private sealed class RegisterUserHandler(
        DataContext dbContext,
        ITokenService tokenService,
        IValidator<RegisterUser> validator)
        : IRequestHandler<RegisterUser, UserDto>
    {
        public async Task<UserDto> Handle(RegisterUser request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                // Optionally, return errors in a structured way, or throw an exception.
                throw new ValidationException(validationResult.Errors);
            }

            using var hmac = new HMACSHA512();

            var user = new User
            {
                Id = default,
                UserName = request.Request.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Request.Password)),
                PasswordSalt = hmac.Key
            };

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new UserDto
            {
                Name = user.UserName,
                Token = tokenService.CreateToken(user)
            };
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
