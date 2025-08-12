using API.Data;
using API.DTOs.Comments;
using API.Entities;
using API.Features.Posts.Commands;
using AutoMapper;

namespace API.Profiles.Resolvers;

public class CreatedByResolver(DataContext dbContext) : IValueResolver<CreateCommentDto, Comment, User>
{
    public User Resolve(CreateCommentDto source, Comment destination, User destMember, ResolutionContext context)
    {
        return dbContext.Users.First(user => user.Id == source.UserId);
    }
}

public class CreatedByCommandResolver(DataContext dbContext) 
    : IValueResolver<CreateCommentCommandRequest, Comment, User>
{
    public User Resolve(CreateCommentCommandRequest source, Comment destination, User destMember, ResolutionContext context)
    {
        return dbContext.Users.First(user => user.Id == source.UserId);
    }
}