using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Comments;

public class UpdateCommentDto
{
    [Required]
    public required string Message { get; set; }
}
