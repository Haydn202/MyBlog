using API.Data;
using API.DTOs.Comments;
using API.DTOs.Posts;
using API.Entities;
using API.Features.Posts.Commands;
using API.Features.Posts.Queries;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class PostsController(
    IMapper mapper,
    DataContext context,
    ISender sender): BaseApiController
{
    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<Results<NotFound, Ok<PostDto>>> GetPost(Guid id)
    {
        var query = new GetPost(id);
        var response = await sender.Send(query);
        
        if (response is null)
        {
            return TypedResults.NotFound();
        }
        
        return TypedResults.Ok(response);
    }
    
    [AllowAnonymous]
    [HttpGet]
    public async Task<Ok<List<PostSummaryDto>>> GetPosts()
    {
        var query = new GetPosts();
        var response = await sender.Send(query);

        return TypedResults.Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<PostSummaryDto>> CreatePost(PostCreateDto request)
    {
        var command = new CreatePost(request);
        var response = await sender.Send(command);

        if (!response.IsSuccess)
        {
            return BadRequest(new
            {
                response.Errors
            });
        }

        return Ok(response.Data);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<Results<NoContent, NotFound>> DeletePost(Guid id)
    {
        var post = await context.Posts.FirstOrDefaultAsync(x => x.Id == id);

        if (post is null)
        {
            return TypedResults.NotFound();
        }

        context.Posts.Remove(post);
        await context.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<Results<Ok<PostSummaryDto>, NotFound>> UpdatePost(PostUpdateDto request, [FromRoute] Guid id)
    {
        var post = await context.Posts.FirstOrDefaultAsync(x => x.Id == id);

        if (post is null)
        {
            return TypedResults.NotFound();
        }

        mapper.Map(request, post);
        
        await context.SaveChangesAsync();
        
        return TypedResults.Ok(mapper.Map<PostSummaryDto>(post));
    }
    
    [AllowAnonymous]
    [HttpGet("{postId:guid}/comments")]
    public async Task<Ok<List<CommentDto>>> GetComments(Guid postId)
    {
        var comments = await context.Comments
            .Where(c => c.PostId == postId)
            .Include(c => c.CreatedBy)
            .ToListAsync();
        
        return TypedResults.Ok(mapper.Map<List<CommentDto>>(comments));
    }
    
    [AllowAnonymous]
    [HttpPost("{postId:guid}/comments")]
    public async Task<Ok<CommentDto>> CreateComment([FromQuery]Guid postId, CreateCommentDto request)
    {
        var comment = mapper.Map<Comment>(request);

        await context.Comments.AddAsync(comment);
        
        await context.SaveChangesAsync();
        
        return TypedResults.Ok(mapper.Map<CommentDto>(comment));
    }
    
    [AllowAnonymous]
    [HttpGet("{postId:guid}/comments/{commentId:guid}")]
    public async Task<Ok<List<CommentDto>>> GetReplies(Guid postId)
    {
        var comments = context.Comments.Where(x => x.PostId == postId).ToListAsync();
        
        return TypedResults.Ok(mapper.Map<List<CommentDto>>(comments));
    }
}
