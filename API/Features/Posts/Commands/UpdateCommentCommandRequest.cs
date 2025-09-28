using System.ComponentModel.DataAnnotations;

namespace API.Features.Posts.Commands;

public class UpdateCommentCommandRequest
{
    [Required]
    public required Guid CommentId { get; set; }
    
    [Required]
    public required string Message { get; set; }
}
