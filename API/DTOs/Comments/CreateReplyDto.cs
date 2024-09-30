namespace API.DTOs.Comments;

public class CreateReplyDto : CreateCommentBaseDto
{
    public Guid CommentId { get; set; }
}