using System.ComponentModel.DataAnnotations;
using API.Entities;

namespace API.Features.Posts.Commands;

public class UpdatePostCommandRequest
{
    [Required]
    public Guid Id { get; set; }
    
    [StringLength(100, MinimumLength = 3)]
    public required string Title { get; set; }
    
    [StringLength(1000, MinimumLength = 3)]
    public required string Description { get; set; } 
    
    public string? ThumbnailUrl { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    
    public required string Content { get; set; }
    public required List<Guid> TopicIds { get; init; }
    
    [EnumDataType(typeof(PostStatus), ErrorMessage = "Status must be one of: Draft, Published, Deleted")]
    public PostStatus Status { get; set; }
}