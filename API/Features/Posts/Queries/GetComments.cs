using API.Data;
using API.DTOs.Comments;
using API.Entities;
using API.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Posts.Queries;

public class GetComments(Guid id, PagingParams pagingParams) : IRequest<PaginatedResult<CommentDto>>
{
    private Guid Id { get; } = id;
    private PagingParams PagingParams { get; } = pagingParams;

    private sealed class GetCommentsHandler(
        DataContext dbContext,
        IMapper mapper) : IRequestHandler<GetComments, PaginatedResult<CommentDto>>
    {
        public async Task<PaginatedResult<CommentDto>> Handle(GetComments request, CancellationToken cancellationToken)
        {
            var comments = await dbContext.Comments
                .Where(c => c.PostId == request.Id)
                .Include(c => c.CreatedBy)
                .Include(c => c.Replies!)
                    .ThenInclude(r => r.CreatedBy)
                .OrderByDescending(c => c.CreatedOn) // Comments: newest first
                .ToListAsync(cancellationToken);
            
            // Map to DTOs manually
            var commentDtos = comments.Select(comment => new CommentDto
            {
                Id = comment.Id,
                Message = comment.Message,
                CreatedOn = comment.CreatedOn,
                UserName = comment.CreatedBy.UserName,
                UserId = comment.CreatedBy.Id,
                Replies = comment.Replies?
                    .OrderBy(r => r.CreatedOn) // Replies: oldest first
                    .Select(reply => new ReplyDto
                    {
                        Id = reply.Id,
                        Message = reply.Message,
                        CreatedOn = reply.CreatedOn,
                        UserName = reply.CreatedBy.UserName,
                        UserId = reply.CreatedBy.Id
                    }).ToList() ?? new List<ReplyDto>()
            }).ToList();
            
            // Apply pagination manually
            var totalCount = commentDtos.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PagingParams.PageSize);
            var pagedItems = commentDtos
                .Skip((request.PagingParams.PageNumber - 1) * request.PagingParams.PageSize)
                .Take(request.PagingParams.PageSize)
                .ToList();
            
            return new PaginatedResult<CommentDto>
            {
                Items = pagedItems,
                Metadata = new PaginationMetadata
                {
                    CurrentPage = request.PagingParams.PageNumber,
                    TotalPages = totalPages,
                    PageSize = request.PagingParams.PageSize,
                    TotalCount = totalCount
                }
            };
        }
    }
}