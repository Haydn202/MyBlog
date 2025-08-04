using API.Data;
using API.DTOs.Topics;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Topics.Commands;

public class DeleteTopic(Guid id) : IRequest<bool>
{
    public Guid Id { get; set; } = id;
    
    private sealed class DeleteTopicHandler(DataContext dbContext) 
        : IRequestHandler<DeleteTopic, bool>
    {
        public async Task<bool> Handle(DeleteTopic request, CancellationToken cancellationToken)
        {
            var topic = await dbContext.Topics.FirstOrDefaultAsync(x =>
                x.Id == request.Id, cancellationToken: cancellationToken);

            if (topic == null)
            {
                return false;
            }
        
            dbContext.Topics.Remove(topic);
            await dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}

public class DeleteTopicValidator : AbstractValidator<DeleteTopic>
{
    private readonly DataContext _dbContext;

    public DeleteTopicValidator(DataContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(u => u.Id)
            .MustAsync(TopicExists)
            .WithMessage("Topic not found.");
    }

    private async Task<bool> TopicExists(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Topics.AnyAsync(t => t.Id == id, cancellationToken);
    }
}