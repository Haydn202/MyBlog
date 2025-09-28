using API.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Posts.Commands;

public class DeleteComment(Guid commentId) : IRequest<bool>
{
    public Guid CommentId { get; set; } = commentId;
    
    private sealed class DeleteCommentHandler(
        DataContext dbContext) : IRequestHandler<DeleteComment, bool>
    {
        public async Task<bool> Handle(DeleteComment request, CancellationToken cancellationToken)
        {
            var comment = await dbContext.Comments.FindAsync(request.CommentId, cancellationToken);

            if (comment == null)
            {
                return false;
            }
            
            dbContext.Comments.Remove(comment);
            await dbContext.SaveChangesAsync(cancellationToken);
            
            return true;
        }
    }
}

public class DeleteCommentValidator : AbstractValidator<DeleteComment>
{
    private readonly DataContext _dbContext;

    public DeleteCommentValidator(DataContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(u => u.CommentId)
            .MustAsync(CommentExists)
            .WithMessage("Comment not found.");
    }

    private async Task<bool> CommentExists(Guid commentId, CancellationToken cancellationToken)
    {
        return await _dbContext.Comments.AnyAsync(c => c.Id == commentId, cancellationToken);
    }
}
