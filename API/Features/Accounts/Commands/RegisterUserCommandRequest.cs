using System.ComponentModel.DataAnnotations;

namespace API.Features.Accounts.Commands;

public class RegisterUserCommandRequest
{
    [Required]
    public required string Username { get; init; }
    
    [Required] 
    [EmailAddress]
    public required string Email { get; init; }
    
    [Required]
    [MinLength(6)]
    public required string Password { get; init; }
} 