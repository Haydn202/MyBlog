using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Topics;

public class TopicCreateDto
{
    [StringLength(50, MinimumLength = 3)]
    public required string Name { get; set; }
}