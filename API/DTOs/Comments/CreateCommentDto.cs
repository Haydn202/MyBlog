namespace API.DTOs.Comments;

public class CreateCommentDto : CreateCommentBaseDto
{
    public Guid PostId { get; set; }
}