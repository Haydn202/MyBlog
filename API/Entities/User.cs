using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class User : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpires { get; set; }
    public Role? Role { get; set; }
}