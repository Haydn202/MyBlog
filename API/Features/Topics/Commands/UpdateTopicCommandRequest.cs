using System.ComponentModel.DataAnnotations;

namespace API.Features.Topics.Commands;

public class UpdateTopicCommandRequest
{
    [Required]
    public Guid Id { get; set; }
    
    [StringLength(50, MinimumLength = 3)]
    public required string Name { get; set; }
} 