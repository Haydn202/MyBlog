using API.Entities.Comments;

namespace API.DTOs.Comments;

public class CommentDto: CommentBaseDto
{
    public List<ReplyDto>? Replies { get; set; }
}