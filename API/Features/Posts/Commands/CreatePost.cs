using API.Data;
using API.DTOs.Posts;
using API.Entities;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Posts.Commands;

public class CreatePost(PostCreateDto request) : IRequest<PostSummaryDto>
{
    public PostCreateDto Request { get; } = request;

    private sealed class CreatePostHandler(
        DataContext dbContext,
        IMapper mapper) : IRequestHandler<CreatePost, PostSummaryDto>
    {
        public async Task<PostSummaryDto> Handle(CreatePost request, CancellationToken cancellationToken)
        {
            var post = mapper.Map<Post>(request.Request);

            dbContext.Posts.Add(post);
            await dbContext.SaveChangesAsync(cancellationToken);

            return mapper.Map<PostSummaryDto>(post);
        }
    }
}

public class CreatePostValidator : AbstractValidator<CreatePost>
{
    private readonly DataContext _dbContext;
    
    public CreatePostValidator(DataContext dbContext)
    {
        _dbContext = dbContext;
        
        RuleFor(u => u.Request)
            .MustAsync(TitleIsUnique)
            .WithMessage("A post already has this title.");
    }
    
    private async Task<bool> TitleIsUnique(PostCreateDto request, CancellationToken cancellationToken)
    {
        return !await _dbContext.Posts
            .AnyAsync(p => p.Title == request.Title, cancellationToken);
    }
}