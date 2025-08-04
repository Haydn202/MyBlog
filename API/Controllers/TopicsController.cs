using API.DTOs.Topics;
using API.Features.Topics.Commands;
using API.Features.Topics.Queries;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class TopicsController(
    IMapper mapper,
    ISender sender): BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<Ok<List<TopicDto>>> GetTopics()
    {
        var query = new GetTopics();
        var response = await sender.Send(query);
        
        return TypedResults.Ok(response);
    }

    [AllowAnonymous]
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

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<TopicDto>> CreateTopic(TopicCreateDto request)
    {
        var command = new CreateTopic(mapper.Map<CreateTopicCommandRequest>(request));
        var response = await sender.Send(command);

        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<Results<NotFound, Ok<TopicDto>>> UpdateTopic(TopicUpdateDto dto, [FromRoute] Guid id)
    {
        var command = new UpdateTopic(mapper.Map<UpdateTopicCommandRequest>((dto, id)));
        var response = await sender.Send(command);
        
        if (response is null)
        {
            return TypedResults.NotFound();
        }
        
        return TypedResults.Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<Results<NotFound, NoContent>> DeleteTopic(Guid id)
    {
        var command = new DeleteTopic(id);
        var response = await sender.Send(command);

        if (response is false)
        {
            return TypedResults.NotFound();
        }
        
        return TypedResults.NoContent();
    }
}