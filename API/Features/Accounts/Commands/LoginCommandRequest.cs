using System.ComponentModel.DataAnnotations;

namespace API.Features.Accounts.Commands;

public class LoginCommandRequest
{
    [Required]
    public required string UserName { get; set; }
    
    [Required]
    public required string Password { get; set; }
}