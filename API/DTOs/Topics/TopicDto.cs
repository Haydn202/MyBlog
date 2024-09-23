namespace API.DTOs.Topics;

public record TopicDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
}