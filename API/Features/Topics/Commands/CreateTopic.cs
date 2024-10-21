using API.Data;
using API.DTOs.Topics;
using API.Entities;
using AutoMapper;
using MediatR;

namespace API.Features.Topics.Commands;

public class CreateTopic(TopicCreateDto request) : IRequest<TopicDto>
{
    private TopicCreateDto Request { get; } = request;

    private sealed class CreateTopicHandler(DataContext dbContext, IMapper mapper)
        : IRequestHandler<CreateTopic, TopicDto>
    {
        public async Task<TopicDto> Handle(CreateTopic command, CancellationToken cancellationToken)
        {
            var topic = mapper.Map<Topic>(command.Request);

            await dbContext.Topics.AddAsync(topic, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return mapper.Map<TopicDto>(topic);
        }
    }
}