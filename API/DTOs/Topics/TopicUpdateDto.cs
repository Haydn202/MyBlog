namespace API.DTOs.Topics;

public record TopicUpdateDto
{
    public required string Name { get; init; }
}