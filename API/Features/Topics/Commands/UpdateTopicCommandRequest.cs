using System.ComponentModel.DataAnnotations;
using API.Entities;

namespace API.Features.Topics.Commands;

public class UpdateTopicCommandRequest
{
    [Required]
    public Guid Id { get; set; }
    
    [StringLength(50, MinimumLength = 3)]
    public required string Name { get; set; }
    
    public TopicColor Color { get; set; } = TopicColor.None;
} 