using System.ComponentModel.DataAnnotations;

namespace API.Features.Topics.Commands;

public class CreateTopicCommandRequest
{
    [StringLength(50, MinimumLength = 3)]
    public required string Name { get; set; }
} 