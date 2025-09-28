using API.Data;
using API.DTOs.Comments;
using API.Entities.Comments;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Posts.Commands;

public class UpdateReply(UpdateReplyCommandRequest request) : IRequest<ReplyDto>
{
    public UpdateReplyCommandRequest Request { get; set; } = request;
    
    private sealed class UpdateReplyHandler(
        DataContext dbContext,
        IMapper mapper) : IRequestHandler<UpdateReply, ReplyDto>
    {
        public async Task<ReplyDto> Handle(UpdateReply request, CancellationToken cancellationToken)
        {
            var reply = await dbContext.Replies
                .Include(r => r.CreatedBy)
                .FirstOrDefaultAsync(r => r.Id == request.Request.ReplyId, cancellationToken);

            if (reply == null)
            {
                throw new InvalidOperationException("Reply not found");
            }

            reply.Message = request.Request.Message;
            await dbContext.SaveChangesAsync(cancellationToken);
            
            return mapper.Map<ReplyDto>(reply);
        }
    }
}

public class UpdateReplyValidator : AbstractValidator<UpdateReply>
{
    private readonly DataContext _dbContext;

    public UpdateReplyValidator(DataContext dbContext)
    {
        _dbContext = dbContext;
        
        RuleFor(u => u.Request.Message)
            .NotEmpty()
            .WithMessage("Message cannot be empty");
        
        RuleFor(u => u.Request.ReplyId)
            .MustAsync(ReplyExists)
            .WithMessage("Reply not found");
    }

    private async Task<bool> ReplyExists(Guid replyId, CancellationToken cancellationToken)
    {
        return await _dbContext.Replies.AnyAsync(r => r.Id == replyId, cancellationToken);
    }
}
