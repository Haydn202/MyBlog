using API.DTOs.Comments;
using API.DTOs.Posts;
using API.Entities;
using API.Features.Posts.Commands;
using API.Profiles.Resolvers;
using AutoMapper;
namespace API.Profiles;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<Post, PostCreateDto>();
        CreateMap<CreatePostCommandRequest, Post>()
            .ForMember(dest => dest.Topics, opt => opt.MapFrom<PostCreateCommandTopicResolver>());
        CreateMap<Post, CreatePostCommandRequest>();
        CreateMap<PostCreateDto, Post>()
            .ForMember(dest => dest.Topics, opt => opt.MapFrom<PostCreateTopicResolver>());
        CreateMap<Post, PostSummaryDto>();
        CreateMap<Post, PostDto>();
        CreateMap<PostUpdateDto, Post>()
            .ForMember(dest => dest.Topics, opt => opt.MapFrom<PostUpdateTopicResolver>());
        CreateMap<UpdatePostCommandRequest, Post>();
        CreateMap<(PostUpdateDto Dto, Guid Id), UpdatePostCommandRequest>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Dto.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Dto.Description))
            .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => src.Dto.ThumbnailUrl))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.Dto.CreatedOn))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Dto.Content))
            .ForMember(dest => dest.TopicIds, opt => opt.MapFrom(src => src.Dto.TopicIds));
        CreateMap<PostCreateDto, CreatePostCommandRequest>();
        CreateMap<CreateCommentDto, CreateCommentCommandRequest>();
        CreateMap<CreateCommentCommandRequest, CreateCommentDto>();
    }
}