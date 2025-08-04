using System.ComponentModel.DataAnnotations;

namespace API.Features.Accounts.Commands;

public class RegisterUserCommandRequest
{
    [Required]
    public required string Username { get; set; }
    
    [Required]
    [MinLength(6)]
    public required string Password { get; set; }
} 