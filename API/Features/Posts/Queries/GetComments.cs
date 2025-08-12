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
            var query = dbContext.Comments
                .Where(c => c.PostId == request.Id)
                .Include(c => c.CreatedBy)
                .ProjectTo<CommentDto>(mapper.ConfigurationProvider)
                .AsQueryable();
            
            return await PaginationHelper.CreateAsync(query, request.PagingParams.PageNumber, request.PagingParams.PageSize);
        }
    }
}