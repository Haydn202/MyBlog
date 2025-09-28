namespace API.Entities;

public class Post
{
    public Guid Id { get; init; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public string? Thumbnail { get; set; } // Base64 encoded image data
    public required List<Topic> Topics { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public required string Content { get; set; } // will change to be in blob storage.
    public List<Comment>? MainComments { get; set; }
    public PostStatus Status { get; set; } = PostStatus.Draft;
}