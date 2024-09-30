using API.Data;
using API.DTOs.Comments;
using API.Entities;
using AutoMapper;

namespace API.Profiles.Resolvers;

public class UserNameResolver(DataContext dbContext) : IValueResolver<Comment, CommentDto, string>
{
    public string Resolve(Comment source, CommentDto destination, string destMember, ResolutionContext context)
    {
        return dbContext.Users.First(user => user.Id == source.CreatedBy.Id).UserName;
    }
}