using System.ComponentModel.DataAnnotations;

namespace API.Features.Accounts.Commands;

public class LoginCommandRequest
{
    [Required]
    public required string Email { get; set; }
    
    [Required]
    public required string Password { get; set; }
}