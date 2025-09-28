using API.Data;
using API.DTOs.Comments;
using API.Entities;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Posts.Commands;

public class UpdateComment(UpdateCommentCommandRequest request) : IRequest<CommentDto>
{
    public UpdateCommentCommandRequest Request { get; set; } = request;
    
    private sealed class UpdateCommentHandler(
        DataContext dbContext,
        IMapper mapper) : IRequestHandler<UpdateComment, CommentDto>
    {
        public async Task<CommentDto> Handle(UpdateComment request, CancellationToken cancellationToken)
        {
            var comment = await dbContext.Comments
                .Include(c => c.CreatedBy)
                .FirstOrDefaultAsync(c => c.Id == request.Request.CommentId, cancellationToken);

            if (comment == null)
            {
                throw new InvalidOperationException("Comment not found");
            }

            comment.Message = request.Request.Message;
            await dbContext.SaveChangesAsync(cancellationToken);
            
            return mapper.Map<CommentDto>(comment);
        }
    }
}

public class UpdateCommentValidator : AbstractValidator<UpdateComment>
{
    private readonly DataContext _dbContext;

    public UpdateCommentValidator(DataContext dbContext)
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
