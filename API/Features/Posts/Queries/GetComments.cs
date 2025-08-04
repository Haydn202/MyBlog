using API.Data;
using API.DTOs.Comments;
using API.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Posts.Queries;

public class GetComments(Guid id) : IRequest<List<CommentDto>>
{
    public Guid Id { get; } = id;

    private sealed class GetCommentsHandler(
        DataContext dbContext,
        IMapper mapper) : IRequestHandler<GetComments, List<CommentDto>>
    {
        public async Task<List<CommentDto>> Handle(GetComments request, CancellationToken cancellationToken)
        {
            var comments = await dbContext.Comments
                .Where(c => c.PostId == request.Id)
                .Include(c => c.CreatedBy)
                .ToListAsync(cancellationToken: cancellationToken);
            
            return mapper.Map<List<CommentDto>>(comments);
        }
    }
}