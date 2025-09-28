using System.ComponentModel.DataAnnotations;

namespace API.Features.Posts.Commands;

public class CreateReplyCommandRequest
{
    [Required]
    public required Guid CommentId { get; set; }
    
    [Required]
    public required string Message { get; set; }
    
    [Required]
    public required string UserId { get; set; }
}
