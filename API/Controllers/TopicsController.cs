using API.Data;
using API.DTOs.Topics;
using API.Entities;
using API.Features.Topics.Commands;
using API.Features.Topics.Queries;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class TopicsController(
    IMapper mapper, 
    DataContext context,
    ISender sender): BaseApiController
{
    [HttpGet]
    public async Task<Ok<List<TopicDto>>> GetTopics()
    {
        var query = new GetTopics();
        var response = await sender.Send(query);
        
        return TypedResults.Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<Results<NotFound, Ok<TopicDto>>> GetTopic(Guid id)
    {
        var query = new GetTopic(id);
        var response = await sender.Send(query);
        
        if (response is null)
        {
            return TypedResults.NotFound();
        }
        
        return TypedResults.Ok(response);
    }

    [HttpPost]
    public async Task<Ok<TopicDto>> CreateTopic(TopicCreateDto request)
    {
        var command = new CreateTopic(request);
        var response = await sender.Send(command);

        return TypedResults.Ok(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<Results<NotFound, Ok<TopicDto>>> UpdateTopic(TopicUpdateDto request, [FromRoute]Guid id)
    {
        var topic = await context.Topics.FirstOrDefaultAsync(x => x.Id == id);

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