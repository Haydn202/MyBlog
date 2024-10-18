using API.Data;
using API.DTOs.Topics;
using API.Entities;
using AutoMapper;
using MediatR;

namespace API.Features.Topics.Commands;

public class CreateTopicCommand(TopicCreateDto request) : IRequest<TopicDto>
{
    private TopicCreateDto Request { get; } = request;

    private sealed class CreateTopicCommandHandler(DataContext dbContext, IMapper mapper)
        : IRequestHandler<CreateTopicCommand, TopicDto>
    {
        public async Task<TopicDto> Handle(CreateTopicCommand command, CancellationToken cancellationToken)
        {
            var topic = mapper.Map<Topic>(command.Request);

            await dbContext.Topics.AddAsync(topic, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return mapper.Map<TopicDto>(topic);
        }
    }
}