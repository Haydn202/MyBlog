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

public class Login(LoginDto request) : IRequest<ValidationResult<UserDto>>
{
    public LoginDto Request { get;} = request;
    
    private sealed class LoginHandler(
        DataContext dbContext, 
        ITokenService tokenService,
        IValidator<Login> validator) 
        : IRequestHandler<Login, ValidationResult<UserDto>>
    {
        public async Task<ValidationResult<UserDto>> Handle(Login request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ValidationResult<UserDto>.Failure(errors);
            }
            
            var user = await dbContext.Users.FirstOrDefaultAsync(u => 
                u.UserName == request.Request.UserName.ToLower());

            var userDto = new UserDto
            {
                Name = user.UserName,
                Token = tokenService.CreateToken(user)
            };

            return ValidationResult<UserDto>.Success(userDto);
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
    
    private async Task<bool> PasswordIsValid(LoginDto request)
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