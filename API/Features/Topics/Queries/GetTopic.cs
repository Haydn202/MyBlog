using API.Data;
using API.DTOs.Topics;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Topics.Queries;

public class GetTopic(Guid topicId) : IRequest<TopicDto>
{
    private Guid TopicId { get; } = topicId;

    private sealed class GetTopicHandler(DataContext dbContext, IMapper mapper) : IRequestHandler<GetTopic, TopicDto?>
    {
        public async Task<TopicDto?> Handle(GetTopic request, CancellationToken cancellationToken)
        {
            var topic = await dbContext.Topics
                .FirstOrDefaultAsync(x => x.Id == request.TopicId, cancellationToken: cancellationToken);
        
            if (topic == null)
            {
                return null;
            }
        
            return mapper.Map<TopicDto>(topic);
        }
    }
}