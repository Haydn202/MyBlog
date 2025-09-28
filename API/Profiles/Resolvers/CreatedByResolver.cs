using API.Data;
using API.DTOs.Comments;
using API.Entities;
using API.Features.Posts.Commands;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace API.Profiles.Resolvers;

public class CreatedByResolver(DataContext dbContext, IHttpContextAccessor httpContextAccessor) : IValueResolver<CreateCommentDto, Comment, User>
{
    public User Resolve(CreateCommentDto source, Comment destination, User destMember, ResolutionContext context)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }
        return dbContext.Users.First(user => user.Id == userId);
    }
}

public class CreatedByCommandResolver(DataContext dbContext, IHttpContextAccessor httpContextAccessor) 
    : IValueResolver<CreateCommentCommandRequest, Comment, User>
{
    public User Resolve(CreateCommentCommandRequest source, Comment destination, User destMember, ResolutionContext context)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }
        return dbContext.Users.First(user => user.Id == userId);
    }
}