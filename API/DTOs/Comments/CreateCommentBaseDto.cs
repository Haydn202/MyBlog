namespace API.DTOs.Comments;

public class CreateCommentBaseDto
{
    public required string Message { get; set; }
    public Guid UserId { get; init; }
}