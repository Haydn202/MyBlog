using System.ComponentModel.DataAnnotations;
using API.DTOs.Topics;
using API.Entities;

namespace API.DTOs.Posts;

public class PostUpdateDto
{
    [StringLength(100, MinimumLength = 3)]
    public required string Title { get; set; }
    
    [StringLength(1000, MinimumLength = 3)]
    public required string Description { get; set; } 
    
    public string? ThumbnailUrl { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    
    public required string Content { get; set; }
    public List<TopicDto> Topics { get; set; }
}