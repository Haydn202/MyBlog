using System.ComponentModel.DataAnnotations;
using API.Entities;

namespace API.Features.Posts.Commands;

public class UpdatePostStatusCommandRequest
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public PostStatus Status { get; set; }
}
