using API.Data;
using API.DTOs;
using API.DTOs.Posts;
using API.Entities;
using AutoMapper;
using FluentValidation;
using MediatR;

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
            .MustAsync(async (request, cancellationToken) => 
                dbContext.Posts.Where(p => p.Title == request.Title).ToList().Count == 0)
            .WithMessage("A post already a has this title.");
    }
}