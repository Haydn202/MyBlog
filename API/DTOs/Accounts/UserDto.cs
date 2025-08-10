using API.Entities;

namespace API.DTOs.Accounts;

public class UserDto
{
    public string Id { get; init; }
    public required string UserName { get; set; }
    public required string Token { get; set; }
}