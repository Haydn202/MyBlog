using API.Data;
using API.DTOs.Topics;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Topics.Commands;

public class UpdateTopic(TopicUpdateDto request, Guid id) : IRequest<TopicDto>
{
    private TopicUpdateDto Request { get; set; } = request;
    private Guid Id { get; set; } = id;
    
    private sealed class UpdateTopicHandler(DataContext dbContext, IMapper mapper) 
        : IRequestHandler<UpdateTopic, TopicDto?>
    {
        public async Task<TopicDto?> Handle(UpdateTopic command, CancellationToken cancellationToken)
        {
            var topic = await dbContext.Topics
                .FirstOrDefaultAsync(x 
                    => x.Id == command.Id, cancellationToken: cancellationToken);

            if (topic is null)
            {
                return null;
            }
            
            mapper.Map(command.Request, topic);
            await dbContext.SaveChangesAsync(cancellationToken);

            return mapper.Map<TopicDto>(topic);
        }
    }
}