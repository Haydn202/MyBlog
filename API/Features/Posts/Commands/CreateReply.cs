using API.Data;
using API.DTOs.Comments;
using API.Entities.Comments;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Posts.Commands;

public class CreateReply(CreateReplyCommandRequest request) : IRequest<ReplyDto>
{
    public CreateReplyCommandRequest Request { get; set; } = request;
    
    private sealed class CreateReplyHandler(
        DataContext dbContext,
        IMapper mapper) : IRequestHandler<CreateReply, ReplyDto>
    {
        public async Task<ReplyDto> Handle(CreateReply request, CancellationToken cancellationToken)
        {
            var reply = mapper.Map<Reply>(request.Request);

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
        
        RuleFor(u => u.Request.UserId)
            .MustAsync(UserExists)
            .WithMessage("User not found");
    }

    private async Task<bool> UserExists(string userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.AnyAsync(u => u.Id == userId, cancellationToken);
    }

    private async Task<bool> CommentExists(Guid commentId, CancellationToken cancellationToken)
    {
        return await _dbContext.Comments.AnyAsync(c => c.Id == commentId, cancellationToken);
    }
}
