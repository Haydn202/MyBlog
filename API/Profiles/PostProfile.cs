using API.DTOs.Posts;
using API.Entities;
using AutoMapper;
namespace API.Profiles;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<Post, PostCreateDto>();
        CreateMap<PostCreateDto, Post>();
        CreateMap<Post, PostSummary>();
        CreateMap<PostUpdateDto, Post>();
    }
}