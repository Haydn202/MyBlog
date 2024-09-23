using API.Entities.Comments;

namespace API.DTOs.Comments;

public class MainCommentDto
{
    public Guid Id { get; set; }
    public string Message { get; set; }
    public DateTime CreatedOn { get; set; }
    public List<SubCommentDto>? SubComments { get; set; }
}