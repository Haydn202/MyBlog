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
            .ForMember(dest => dest.Topics, opt => opt.MapFrom<TopicResolver>());
        CreateMap<Post, PostSummary>();
        CreateMap<Post, PostDto>();
        CreateMap<PostUpdateDto, Post>();
    }
}