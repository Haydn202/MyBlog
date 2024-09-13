using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Accounts;

public class RegisterDto
{
    [Required]
    public required string Username { get; set; }
    
    [Required]
    public required string Password { get; set; }
}