using API.Data;
using API.DTOs.Topics;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Topics.Commands;

public class UpdateTopic(UpdateTopicCommandRequest request) : IRequest<TopicDto?>
{
    public UpdateTopicCommandRequest Request { get; set; } = request;

    private sealed class UpdateTopicHandler(DataContext dbContext, IMapper mapper) 
        : IRequestHandler<UpdateTopic, TopicDto?>
    {
        public async Task<TopicDto?> Handle(UpdateTopic request, CancellationToken cancellationToken)
        {
            var topic = await dbContext.Topics
                .FirstOrDefaultAsync(x => x.Id == request.Request.Id, cancellationToken);

            if (topic is null)
            {
                return null;
            }
            
            mapper.Map(request.Request, topic);
            await dbContext.SaveChangesAsync(cancellationToken);

            return mapper.Map<TopicDto>(topic);
        }
    }
}

public class UpdateTopicValidator : AbstractValidator<UpdateTopic>
{
    private readonly DataContext _dbContext;

    public UpdateTopicValidator(DataContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(u => u.Request)
            .MustAsync(TopicExists)
            .WithMessage("Topic not found.");

        RuleFor(u => u.Request)
            .MustAsync(NameIsUniqueUnlessSameTopic)
            .WithMessage("A topic already has this name.");
    }

    private async Task<bool> TopicExists(UpdateTopicCommandRequest request, CancellationToken cancellationToken)
    {
        return await _dbContext.Topics.AnyAsync(t => t.Id == request.Id, cancellationToken);
    }

    private async Task<bool> NameIsUniqueUnlessSameTopic(UpdateTopicCommandRequest request, CancellationToken cancellationToken)
    {
        return !await _dbContext.Topics
            .AnyAsync(t => t.Name == request.Name && t.Id != request.Id, cancellationToken);
    }
}