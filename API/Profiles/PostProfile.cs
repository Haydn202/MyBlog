using API.DTOs.Posts;
using API.Entities;
using API.Profiles.Resolvers;
using AutoMapper;
namespace API.Profiles;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<Post, PostCreateDto>();
        CreateMap<PostCreateDto, Post>()
            .ForMember(dest => dest.Topics, opt => opt.MapFrom<PostCreateTopicResolver>());
        CreateMap<Post, PostSummaryDto>();
        CreateMap<Post, PostDto>();
        CreateMap<PostUpdateDto, Post>()
            .ForMember(dest => dest.Topics, opt => opt.MapFrom<PostUpdateTopicResolver>());
    }
}