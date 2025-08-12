using API.Data;
using API.DTOs.Comments;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace API.Profiles.Resolvers;

public class UserNameResolver(UserManager<User> userManager) : IValueResolver<Comment, CommentDto, string>
{
    public string Resolve(Comment source, CommentDto destination, string destMember,
        ResolutionContext context)
    {
        var user = userManager.FindByIdAsync(source.CreatedBy.Id).Result;
        
        return user.UserName;
    }
}
