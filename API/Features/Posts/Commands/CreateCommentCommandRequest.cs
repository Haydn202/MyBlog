using System.ComponentModel.DataAnnotations;

namespace API.Features.Posts.Commands;

public class CreateCommentCommandRequest
{
    [Required]
    public required string Message { get; set; }
} 