﻿namespace API.Entities.Comments;

public class CommentBase
{
    public Guid Id { get; init; }
    public required string Message { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public required User CreatedBy { get; init; }
}