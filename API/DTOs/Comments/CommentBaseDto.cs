namespace API.DTOs.Comments;

public class CommentBaseDto
{
    public required string Message { get; set; }
    public DateTime CreatedOn { get; set; }
    public required string UserName { get; init; }
}