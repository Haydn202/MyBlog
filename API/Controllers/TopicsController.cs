using API.Data;
using API.DTOs.Topics;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class TopicsController(IMapper mapper, DataContext context): BaseApiController
{
    [HttpGet]
    public async Task<List<Topic>> GetTopics()
    {
        return await context.Topics.ToListAsync();
    }

    [HttpGet("{id:guid}")]
    public async Task<Ok<TopicDto>> GetTopic(Guid id)
    {
        var topic = await context.Topics.FirstAsync(x => x.Id == id);
        
        return TypedResults.Ok(mapper.Map<TopicDto>(topic));
    }

    [HttpPost]
    public async Task<Ok<TopicDto>> CreateTopic(TopicCreateDto request)
    {
        var topic = mapper.Map<Topic>(request);

        await context.Topics.AddAsync(topic);
        await context.SaveChangesAsync();

        return TypedResults.Ok(mapper.Map<TopicDto>(topic));
    }

    [HttpPut]
    public async Task<Results<NotFound, Ok<TopicDto>>> UpdateTopic(TopicUpdateDto request)
    {
        var topic = await context.Topics.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (topic == null)
        {
            return TypedResults.NotFound();
        }
        
        mapper.Map(request, topic);
        await context.SaveChangesAsync();
        
        return TypedResults.Ok(mapper.Map<TopicDto>(topic));
    }

    [HttpDelete("{id:guid}")]
    public async Task<Results<NotFound, NoContent>> DeleteTopic(Guid id)
    {
        var topic = await context.Topics.FirstOrDefaultAsync(x => x.Id == id);

        if (topic == null)
        {
            return TypedResults.NotFound();
        }
        
        context.Topics.Remove(topic);

        await context.SaveChangesAsync();
        return TypedResults.NoContent();
    }
}