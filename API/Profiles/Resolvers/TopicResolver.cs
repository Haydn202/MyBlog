using API.Data;
using API.DTOs.Posts;
using API.Entities;
using AutoMapper;

namespace API.Profiles.Resolvers;

public class TopicResolver : IValueResolver<PostCreateDto, Post, List<Topic>>
{
    private readonly DataContext _dbContext;

    public TopicResolver(DataContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Topic> Resolve(
        PostCreateDto source, 
        Post destination, 
        List<Topic> destMember, 
        ResolutionContext context)
    {
        return _dbContext.Topics
            .Where(t => source.TopicIds.Contains(t.Id))
            .ToList();
    }
}
