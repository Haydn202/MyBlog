using API.Entities;

namespace API.DTOs.Accounts;

public class UserDto
{
    public Guid Id { get; init; }
    public required string UserName { get; set; }
    public required string Token { get; set; }
    public string? Role { get; set; }
}