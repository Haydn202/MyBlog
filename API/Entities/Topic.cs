namespace API.Entities;

public class Topic
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public List<Post> Posts { get; set; }
}