using API.Data;
using API.DTOs.Topics;
using API.Entities;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Topics.Commands;

public class CreateTopic(CreateTopicCommandRequest request) : IRequest<TopicDto>
{
    public CreateTopicCommandRequest Request { get; set; } = request;

    private sealed class CreateTopicHandler(DataContext dbContext, IMapper mapper)
        : IRequestHandler<CreateTopic, TopicDto>
    {
        public async Task<TopicDto> Handle(CreateTopic request, CancellationToken cancellationToken)
        {
            var topic = mapper.Map<Topic>(request.Request);

            await dbContext.Topics.AddAsync(topic, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return mapper.Map<TopicDto>(topic);
        }
    }
}

public class CreateTopicValidator : AbstractValidator<CreateTopic>
{
    private readonly DataContext _dbContext;

    public CreateTopicValidator(DataContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(u => u.Request)
            .MustAsync(NameIsUnique)
            .WithMessage("A topic already has this name.");
    }

    private async Task<bool> NameIsUnique(CreateTopicCommandRequest request, CancellationToken cancellationToken)
    {
        return !await _dbContext.Topics
            .AnyAsync(t => t.Name == request.Name, cancellationToken);
    }
}