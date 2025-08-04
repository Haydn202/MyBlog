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
        
        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<Results<NoContent, NotFound>> DeletePost(Guid id)
    {
        var command = new DeletePost(id);
        var response = await sender.Send(command);

        if (response is false)
        {
            return TypedResults.NotFound();
        }
        
        return TypedResults.NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PostSummaryDto>> UpdatePost(PostUpdateDto dto, [FromRoute] Guid id)
    {
        var command = new UpdatePost(mapper.Map<UpdatePostCommandRequest>((dto, id)));
        var response = await sender.Send(command);
        
        return Ok(response);
    }
    
    [AllowAnonymous]
    [HttpGet("{postId:guid}/comments")]
    public async Task<Ok<List<CommentDto>>> GetComments(Guid postId)
    {
        var query = new GetComments(postId);
        var response = await sender.Send(query);
        return TypedResults.Ok(mapper.Map<List<CommentDto>>(response));
    }
    
    [AllowAnonymous]
    [HttpPost("{postId:guid}/comments")]
    public async Task<ActionResult<CommentDto>> CreateComment([FromQuery]Guid postId, CreateCommentDto request)
    {
        var command = new CreateComment(request);
        var response = await sender.Send(command);
        
        return Ok(response);
    }
    
    // TODO add logic for replies.
}
