using API.Data;
using API.DTOs.Comments;
using API.Entities;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Posts.Commands;

public class CreateComment(CreateCommentDto request) : IRequest<CommentDto>
{
    public CreateCommentDto Request { get; } = request;
    
    private sealed class CreateCommentHandler(
        DataContext dbContext,
        IMapper mapper) : IRequestHandler<CreateComment, CommentDto>
    {
        public async Task<CommentDto> Handle(CreateComment request, CancellationToken cancellationToken)
        {
            var comment = mapper.Map<Comment>(request);

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
        
        RuleFor(u => u.Request)
            .MustAsync(PostExists)
            .WithMessage("Post not found");
    }

    private async Task<bool> PostExists(CreateCommentDto request,  CancellationToken cancellationToken)
    {
        return await _dbContext.Posts
            .AnyAsync(post => post.Id == request.PostId, cancellationToken: cancellationToken);
    }
}