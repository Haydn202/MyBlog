using API.Entities.Comments;

namespace API.Entities;

public class Comment : CommentBase
{
    public Guid PostId { get; set; }
    public List<Reply>? Replies { get; set; }
}