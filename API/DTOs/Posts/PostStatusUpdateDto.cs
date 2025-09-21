using System.ComponentModel.DataAnnotations;
using API.Entities;

namespace API.DTOs.Posts;

public class PostStatusUpdateDto
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    [EnumDataType(typeof(PostStatus), ErrorMessage = "Status must be one of: Draft, Published, Deleted")]
    public PostStatus Status { get; set; }
}
