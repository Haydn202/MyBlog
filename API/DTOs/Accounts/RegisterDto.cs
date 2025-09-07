using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Accounts;

public class RegisterDto
{
    [Required]
    public required string UserName { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public required string Password { get; set; }
}