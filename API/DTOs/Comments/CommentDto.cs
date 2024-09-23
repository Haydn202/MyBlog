namespace API.DTOs.Comments;

public class CommentDto
{
    public Guid Id { get; init; }
    public required string Message { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid UserId { get; init; }
}