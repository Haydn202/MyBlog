namespace API.Entities;

public class User
{
    public Guid Id { get; init; }
    public required string UserName { get; set; }
    public required byte[] PasswordHash { get; set; }
    public required byte[] PasswordSalt { get; set; }
}