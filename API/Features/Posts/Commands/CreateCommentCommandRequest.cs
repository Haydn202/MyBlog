using System.ComponentModel.DataAnnotations;

namespace API.Features.Posts.Commands;

public class CreateCommentCommandRequest
{
    [Required]
    public required Guid PostId { get; set; }
    
    [Required]
    public required string Message { get; set; }
    
    [Required]
    public required string UserId { get; set; }
} 