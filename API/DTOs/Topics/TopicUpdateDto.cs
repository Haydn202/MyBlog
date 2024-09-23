namespace API.DTOs.Topics;

public record TopicUpdateDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}