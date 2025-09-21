using System.ComponentModel.DataAnnotations;
using API.Entities;

namespace API.Features.Posts.Commands;

public class UpdatePostStatusCommandRequest
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    [EnumDataType(typeof(PostStatus), ErrorMessage = "Status must be one of: Draft, Published, Deleted")]
    public PostStatus Status { get; set; }
}
