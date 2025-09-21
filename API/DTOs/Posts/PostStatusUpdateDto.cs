using System.ComponentModel.DataAnnotations;
using API.Entities;

namespace API.DTOs.Posts;

public class PostStatusUpdateDto
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public PostStatus Status { get; set; }
}
