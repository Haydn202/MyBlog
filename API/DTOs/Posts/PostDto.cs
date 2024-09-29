using API.DTOs.Topics;
using API.Entities;

namespace API.DTOs.Posts;

public class PostDto
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public List<TopicDto> Topics { get; init; }
    public DateTime CreatedOn { get; init; }
    public required string Content { get; init; }
    public List<MainComment>? MainComments { get; init; }
    public string ThumbnailUrl { get; set; }
}