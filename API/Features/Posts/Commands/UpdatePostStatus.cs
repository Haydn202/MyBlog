using API.Data;
using API.DTOs.Posts;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Posts.Commands;

public class UpdatePostStatus(UpdatePostStatusCommandRequest request) : IRequest<PostSummaryDto>
{
    public UpdatePostStatusCommandRequest Request { get; set; } = request;

    private sealed class UpdatePostStatusHandler(
        DataContext dbContext,
        IMapper mapper) : IRequestHandler<UpdatePostStatus, PostSummaryDto>
    {
        public async Task<PostSummaryDto> Handle(UpdatePostStatus request, CancellationToken cancellationToken)
        {
            var post = await dbContext.Posts.FirstOrDefaultAsync(x => x.Id == request.Request.Id, cancellationToken);
            
            if (post == null)
            {
                throw new InvalidOperationException("Post not found.");
            }
            
            post.Status = request.Request.Status;
            
            await dbContext.SaveChangesAsync(cancellationToken);
            
            return mapper.Map<PostSummaryDto>(post);
        }
    }
}

public class UpdatePostStatusValidator : AbstractValidator<UpdatePostStatus>
{
    private readonly DataContext _dbContext;

    public UpdatePostStatusValidator(DataContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(u => u.Request)
            .MustAsync(PostExists)
            .WithMessage("Post not found.");
    }

    private async Task<bool> PostExists(UpdatePostStatusCommandRequest request, CancellationToken cancellationToken)
    {
        return await _dbContext.Posts.AnyAsync(p => p.Id == request.Id, cancellationToken);
    }
}
