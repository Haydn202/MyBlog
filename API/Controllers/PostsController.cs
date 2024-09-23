using API.Data;
using API.DTOs.Posts;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class PostsController(IMapper mapper, DataContext context): BaseApiController
{
    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<Results<NotFound, Ok<Post>>> GetPost(Guid id)
    {
        var post = await context.Posts.FirstOrDefaultAsync(x => x.Id == id);

        if (post is null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(post);
    }
    
    [AllowAnonymous]
    [HttpGet]
    public async Task<Ok<List<PostSummary>>> GetPosts()
    {
        var posts = await context.Posts
            .OrderByDescending(x => x.CreatedOn)
            .Take(10)
            .ToListAsync();

        return TypedResults.Ok(mapper.Map<List<PostSummary>>(posts));
    }

    [HttpPost]
    public async Task<Ok<PostSummary>> CreatePost(PostCreateDto request)
    {
        var post = mapper.Map<Post>(request);

        context.Posts.Add(post);
        await context.SaveChangesAsync();

        return TypedResults.Ok(mapper.Map<PostSummary>(post));
    }

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
}