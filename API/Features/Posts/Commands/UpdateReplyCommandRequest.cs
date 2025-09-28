using System.ComponentModel.DataAnnotations;

namespace API.Features.Posts.Commands;

public class UpdateReplyCommandRequest
{
    [Required]
    public required Guid ReplyId { get; set; }
    
    [Required]
    public required string Message { get; set; }
}
