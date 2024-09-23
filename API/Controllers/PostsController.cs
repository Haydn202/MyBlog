using API.Data;
using API.DTOs.Posts;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class PostsController(IMapper mapper, DataContext context): BaseApiController
{
    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Post>> GetPost(Guid id)
    {
        return await context.Posts.FindAsync(id);
    }
    
    [AllowAnonymous]
    [HttpGet]
    public async Task<List<Post>> GetPosts()
    {
        return await context.Posts
            .OrderByDescending(x => x.CreatedOn)
            .Take(10)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Post>> CreatePost(PostCreateDto request)
    {
        var post = mapper.Map<Post>(request);

        context.Posts.Add(post);
        await context.SaveChangesAsync();

        return post;
    }
}