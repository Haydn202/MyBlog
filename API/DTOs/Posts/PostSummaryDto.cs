using API.DTOs.Topics;
using API.Entities;
using System.Text.Json.Serialization;

namespace API.DTOs.Posts;

public class PostSummaryDto
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required List<TopicDto> Topics { get; set; }
    public DateTime CreatedOn { get; init; }
    public string? Thumbnail { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PostStatus Status { get; init; }
}