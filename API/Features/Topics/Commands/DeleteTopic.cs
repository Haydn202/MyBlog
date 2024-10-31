using API.Data;
using API.DTOs.Topics;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Topics.Commands;

public class DeleteTopic(Guid id) : IRequest<TopicDto>
{
    private Guid Id { get; set; } = id;
    
    private sealed class DeleteTopicHandler(DataContext dbContext, IMapper mapper) 
        : IRequestHandler<DeleteTopic, TopicDto?>
    {
        public async Task<TopicDto?> Handle(DeleteTopic command, CancellationToken cancellationToken)
        {
            var topic = await dbContext.Topics.FirstOrDefaultAsync(x =>
                x.Id == command.Id, cancellationToken: cancellationToken);

            if (topic == null)
            {
                return null;
            }
            
            var topicDto = mapper.Map<TopicDto>(topic);
        
            dbContext.Topics.Remove(topic);

            await dbContext.SaveChangesAsync(cancellationToken);

            return topicDto;
        }
    }
}