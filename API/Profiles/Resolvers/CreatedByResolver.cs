using API.Data;
using API.DTOs.Comments;
using API.Entities;
using AutoMapper;

namespace API.Profiles.Resolvers;

public class CreatedByResolver(DataContext dbContext) : IValueResolver<CreateCommentDto, Comment, User>
{
    public User Resolve(CreateCommentDto source, Comment destination, User destMember, ResolutionContext context)
    {
        return dbContext.Users.First(user => user.Id == source.UserId);
    }
}