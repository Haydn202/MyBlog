using API.Data;
using API.DTOs.Comments;
using API.Entities;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Features.Posts.Commands;

public class CreateComment(Guid postId, string userId, CreateCommentCommandRequest request) : IRequest<CommentDto>
{
    public CreateCommentCommandRequest Request { get; set; } = request;
    public Guid PostId { get; set; } = postId;
    public string UserId { get; set; } = userId;
    
    private sealed class CreateCommentHandler(
        DataContext dbContext,
        IMapper mapper) : IRequestHandler<CreateComment, CommentDto>
    {
        public async Task<CommentDto> Handle(CreateComment request, CancellationToken cancellationToken)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }
            
            var comment = new Comment
            {
                Message = request.Request.Message,
                PostId = request.PostId,
                CreatedBy = user,
                CreatedOn = DateTime.UtcNow
            };

            await dbContext.Comments.AddAsync(comment, cancellationToken);
        
            await dbContext.SaveChangesAsync(cancellationToken);
            
            return mapper.Map<CommentDto>(comment);
        }
    }
}

public class CreateCommentValidator : AbstractValidator<CreateComment>
{
    private readonly DataContext _dbContext;

    public CreateCommentValidator(DataContext dbContext)
    {
        _dbContext = dbContext;
        
        RuleFor(u => u.Request.Message)
            .NotEmpty()
            .WithMessage("Message cannot be empty");
        
        RuleFor(u => u.PostId)
            .MustAsync(PostExists)
            .WithMessage("Post not found");
    }

    private async Task<bool> PostExists(Guid postId, CancellationToken cancellationToken)
    {
        return await _dbContext.Posts
            .AnyAsync(post => post.Id == postId, cancellationToken: cancellationToken);
    }
}