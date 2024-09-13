using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Accounts;

public class LoginDto
{
    [Required]
    public required string UserName { get; set; }
    
    [Required]
    public required string Password { get; set; }
}