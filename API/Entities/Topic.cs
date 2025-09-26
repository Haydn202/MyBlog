namespace API.Entities;

public class Topic
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public TopicColor Color { get; set; } = TopicColor.None;
    public required List<Post> Posts { get; set; }
}