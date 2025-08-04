using API.Data;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Posts.Commands;

public class DeletePost(Guid postId) : IRequest<bool>
{
    public Guid PostId { get; set; } = postId;
    
    private sealed class DeletePostHandler(
        DataContext dbContext) : IRequestHandler<DeletePost, bool>
    {
        public async Task<bool> Handle(DeletePost request, CancellationToken cancellationToken)
        {
            var post = await dbContext.Posts.FindAsync(request.PostId, cancellationToken);

            if (post == null)
            {
                return false;
            }
            
            dbContext.Posts.Remove(post);
            await dbContext.SaveChangesAsync(cancellationToken);
            
            return true;
        }
    }
}

public class DeletePostValidator : AbstractValidator<DeletePost>
{
    private readonly DataContext _dbContext;

    public DeletePostValidator(DataContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(u => u.PostId)
            .MustAsync(PostExists)
            .WithMessage("Post not found.");
    }

    private async Task<bool> PostExists(Guid postId, CancellationToken cancellationToken)
    {
        return await _dbContext.Posts.AnyAsync(p => p.Id == postId, cancellationToken);
    }
}