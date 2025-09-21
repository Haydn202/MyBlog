using API.DTOs.Topics;
using API.Entities;

namespace API.DTOs.Posts;

public class PostDto
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required List<TopicDto> Topics { get; init; }
    public DateTime CreatedOn { get; init; }
    public required string Content { get; init; }
    public List<Comment>? MainComments { get; init; }
    public string? ThumbnailUrl { get; set; }
    public PostStatus Status { get; init; }
}