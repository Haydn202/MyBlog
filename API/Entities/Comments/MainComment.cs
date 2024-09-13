using API.Entities.Comments;

namespace API.Entities;

public class MainComment : Comment
{
    public Guid PostId { get; set; }
    public List<SubComment>? SubComments { get; set; }
}