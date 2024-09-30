using API.DTOs.Comments;
using API.Entities;
using API.Entities.Comments;
using API.Profiles.Resolvers;
using AutoMapper;

namespace API.Profiles;

public class CommentProfile: Profile
{
    public CommentProfile()
    {
        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom<UserNameResolver>());
        CreateMap<CommentDto, Comment>();
        CreateMap<Reply, ReplyDto>();
        CreateMap<CreateCommentDto, Comment>()
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom<CreatedByResolver>());
    }
}