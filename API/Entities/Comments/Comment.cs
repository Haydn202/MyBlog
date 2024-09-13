namespace API.Entities.Comments;

public class Comment
{
    public Guid Id { get; init; }    
    public required string Message { get; set; }
    public DateTime Created { get; set; }
}