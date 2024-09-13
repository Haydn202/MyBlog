namespace API.Entities.Comments;

public class SubComment : Comment
{
    public Guid MainCommentId { get; set; }
}