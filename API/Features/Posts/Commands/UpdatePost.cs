using API.Data;
using API.DTOs.Posts;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Posts.Commands;

public class UpdatePost(UpdatePostCommandRequest request) : IRequest<PostSummaryDto>
{
    public UpdatePostCommandRequest Request { get; set; } = request;

    private sealed class UpdatePostHandler (
        DataContext dbContext,
        IMapper mapper) : IRequestHandler<UpdatePost, PostSummaryDto>
    {
        public async Task<PostSummaryDto> Handle(UpdatePost request, CancellationToken cancellationToken)
        {
            var post = await dbContext.Posts
                .Include(p => p.Topics)
                .FirstOrDefaultAsync(x => x.Id == request.Request.Id, cancellationToken);
            
            if (post == null)
            {
                throw new InvalidOperationException("Post not found.");
            }
            
            // Update basic properties
            post.Title = request.Request.Title;
            post.Description = request.Request.Description;
            post.Thumbnail = request.Request.Thumbnail;
            post.Content = request.Request.Content;
            post.Status = request.Request.Status;
            
            // Update topics
            var topics = await dbContext.Topics
                .Where(t => request.Request.TopicIds.Contains(t.Id))
                .ToListAsync(cancellationToken);
            
            post.Topics = topics;
            
            await dbContext.SaveChangesAsync(cancellationToken);
            
            return mapper.Map<PostSummaryDto>(post);
        }
    }
}

public class UpdatePostValidator : AbstractValidator<UpdatePost>
{
    private readonly DataContext _dbContext;

    public UpdatePostValidator(DataContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(u => u.Request)
            .MustAsync(PostExists)
            .WithMessage("Post not found.");

        RuleFor(u => u.Request)
            .MustAsync(TitleIsUniqueUnlessSamePost)
            .WithMessage("A post already has this title.");
    }

    private async Task<bool> PostExists(UpdatePostCommandRequest request, CancellationToken cancellationToken)
    {
        return await _dbContext.Posts.AnyAsync(p => p.Id == request.Id, cancellationToken);
    }

    private async Task<bool> TitleIsUniqueUnlessSamePost(UpdatePostCommandRequest request, CancellationToken cancellationToken)
    {
        return !await _dbContext.Posts
            .AnyAsync(p => p.Title == request.Title && p.Id != request.Id, cancellationToken);
    }
}
