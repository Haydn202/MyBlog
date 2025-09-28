using API.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Posts.Commands;

public class DeleteReply(Guid replyId) : IRequest<bool>
{
    public Guid ReplyId { get; set; } = replyId;
    
    private sealed class DeleteReplyHandler(
        DataContext dbContext) : IRequestHandler<DeleteReply, bool>
    {
        public async Task<bool> Handle(DeleteReply request, CancellationToken cancellationToken)
        {
            var reply = await dbContext.Replies.FindAsync(request.ReplyId, cancellationToken);

            if (reply == null)
            {
                return false;
            }
            
            dbContext.Replies.Remove(reply);
            await dbContext.SaveChangesAsync(cancellationToken);
            
            return true;
        }
    }
}

public class DeleteReplyValidator : AbstractValidator<DeleteReply>
{
    private readonly DataContext _dbContext;

    public DeleteReplyValidator(DataContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(u => u.ReplyId)
            .MustAsync(ReplyExists)
            .WithMessage("Reply not found.");
    }

    private async Task<bool> ReplyExists(Guid replyId, CancellationToken cancellationToken)
    {
        return await _dbContext.Replies.AnyAsync(r => r.Id == replyId, cancellationToken);
    }
}
