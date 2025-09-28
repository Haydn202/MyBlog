using API.DTOs.Comments;
using API.Entities;
using API.Entities.Comments;
using API.Features.Posts.Commands;
using API.Profiles.Resolvers;
using AutoMapper;

namespace API.Profiles;

public class CommentProfile: Profile
{
    public CommentProfile()
    {
        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.CreatedBy.UserName));
        CreateMap<CommentDto, Comment>();
        CreateMap<Reply, ReplyDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.CreatedBy.UserName));
        CreateMap<CreateCommentDto, Comment>()
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom<CreatedByResolver>());
        CreateMap<CreateCommentCommandRequest, Comment>()
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom<CreatedByCommandResolver>());
        CreateMap<CreateReplyDto, Reply>();
        CreateMap<CreateReplyCommandRequest, Reply>();
    }
}