using API.Data;
using API.DTOs.Comments;
using API.Entities.Comments;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Posts.Commands;

public class CreateReply(string userId, CreateReplyCommandRequest request) : IRequest<ReplyDto>
{
    public CreateReplyCommandRequest Request { get; set; } = request;
    public string UserId { get; set; } = userId;
    
    private sealed class CreateReplyHandler(
        DataContext dbContext,
        IMapper mapper) : IRequestHandler<CreateReply, ReplyDto>
    {
        public async Task<ReplyDto> Handle(CreateReply request, CancellationToken cancellationToken)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            var comment = await dbContext.Comments.FirstAsync(c => c.Id == request.Request.CommentId, cancellationToken);
            
            var reply = new Reply
            {
                Message = request.Request.Message,
                Comment = comment,
                CreatedBy = user,
                CreatedOn = DateTime.UtcNow
            };

            await dbContext.Replies.AddAsync(reply, cancellationToken);
        
            await dbContext.SaveChangesAsync(cancellationToken);
            
            return mapper.Map<ReplyDto>(reply);
        }
    }
}

public class CreateReplyValidator : AbstractValidator<CreateReply>
{
    private readonly DataContext _dbContext;

    public CreateReplyValidator(DataContext dbContext)
    {
        _dbContext = dbContext;
        
        RuleFor(u => u.Request.Message)
            .NotEmpty()
            .WithMessage("Message cannot be empty");
        
        RuleFor(u => u.Request.CommentId)
            .MustAsync(CommentExists)
            .WithMessage("Comment not found");
    }

    private async Task<bool> CommentExists(Guid commentId, CancellationToken cancellationToken)
    {
        return await _dbContext.Comments.AnyAsync(c => c.Id == commentId, cancellationToken);
    }
}
