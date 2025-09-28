using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Comments;

public class UpdateReplyDto
{
    [Required]
    public required string Message { get; set; }
}
