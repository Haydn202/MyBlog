using API.Data;
using API.DTOs.Posts;
using API.Entities;
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
