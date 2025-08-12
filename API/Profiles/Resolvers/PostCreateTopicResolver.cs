using API.Data;
using API.DTOs.Posts;
using API.Entities;
using API.Features.Posts.Commands;
using AutoMapper;

namespace API.Profiles.Resolvers;

public class PostCreateTopicResolver(DataContext dbContext) : IValueResolver<PostCreateDto, Post, List<Topic>>
{
    public List<Topic> Resolve(
        PostCreateDto source, 
        Post destination, 
        List<Topic> destMember, 
        ResolutionContext context)
    {
        return dbContext.Topics
            .Where(t => source.TopicIds.Contains(t.Id))
            .ToList();
    }
}

public class PostCreateCommandTopicResolver(DataContext dbContext) 
    : IValueResolver<CreatePostCommandRequest, Post, List<Topic>>
{
    public List<Topic> Resolve(
        CreatePostCommandRequest source, 
        Post destination, 
        List<Topic> destMember, 
        ResolutionContext context)
    {
        return dbContext.Topics
            .Where(t => source.TopicIds.Contains(t.Id))
            .ToList();
    }
}
