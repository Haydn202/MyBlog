namespace API.DTOs.Comments;

public class CreateCommentBaseDto
{
    public required string Message { get; set; }
    public string UserId { get; init; }
}