using API.Data;
using API.DTOs.Comments;
using API.DTOs.Posts;
using API.Entities;
using API.Features.Posts.Commands;
using API.Features.Posts.Queries;
using API.Helpers;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class PostsController(
    IMapper mapper,
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
    public async Task<Ok<PaginatedResult<PostSummaryDto>>> GetPosts([FromQuery]PagingParams pagingParams)
    {
        var query = new GetPosts(pagingParams);
        var response = await sender.Send(query);

        return TypedResults.Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<PostSummaryDto>> CreatePost(PostCreateDto request)
    {
        var command = new CreatePost(mapper.Map<CreatePostCommandRequest>(request));
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

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<PostSummaryDto>> UpdatePostStatus(PostStatusUpdateDto dto, [FromRoute] Guid id)
    {
        var commandRequest = new UpdatePostStatusCommandRequest { Id = id, Status = dto.Status };
        var command = new UpdatePostStatus(commandRequest);
        var response = await sender.Send(command);
        
        return Ok(response);
    }
    
    [AllowAnonymous]
    [HttpGet("{postId:guid}/comments")]
    public async Task<Ok<PaginatedResult<CommentDto>>> GetComments(Guid postId,  [FromQuery]PagingParams pagingParams)
    {
        var query = new GetComments(postId,  pagingParams);
        var response = await sender.Send(query);
        return TypedResults.Ok(response);
    }
    
    [AllowAnonymous]
    [HttpPost("{postId:guid}/comments")]
    public async Task<ActionResult<CommentDto>> CreateComment(CreateCommentDto request)
    {
        var commandRequest = mapper.Map<CreateCommentCommandRequest>(request);
        var command = new CreateComment(commandRequest);
        var response = await sender.Send(command);
        
        return Ok(response);
    }
    
    // TODO add logic for replies.
}
