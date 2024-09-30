﻿namespace API.Entities;

public class Post
{
    public Guid Id { get; init; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public string ThumbnailUrl { get; set; }
    public List<Topic> Topics { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public required string Content { get; set; }
    public List<Comment>? MainComments { get; set; }
}