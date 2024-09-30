namespace API.Entities.Comments;

public class Reply : CommentBase
{
    public required Comment Comment { get; set; }
}