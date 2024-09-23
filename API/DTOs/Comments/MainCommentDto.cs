using API.Entities.Comments;

namespace API.DTOs.Comments;

public class MainCommentDto: CommentDto
{
    public List<SubCommentDto>? SubComments { get; set; }
}