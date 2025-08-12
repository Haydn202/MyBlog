using API.Data;
using API.DTOs.Posts;
using API.Entities;
using API.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Posts.Queries;

public class GetPosts(PagingParams pagingParams) : IRequest<PaginatedResult<PostSummaryDto>>
{
    private PagingParams PagingParams { get; } = pagingParams;

    private sealed class GetPostsHandler(
        DataContext dbContext, 
        IMapper mapper) : IRequestHandler<GetPosts, PaginatedResult<PostSummaryDto>>
    {
        public async Task<PaginatedResult<PostSummaryDto>> Handle(GetPosts request, CancellationToken cancellationToken)
        {
            var query = dbContext.Posts
                .OrderByDescending(x => x.CreatedOn)
                .Take(10)
                .Include("Topics")
                .ProjectTo<PostSummaryDto>(mapper.ConfigurationProvider)
                .AsQueryable();
            
            return await PaginationHelper.CreateAsync(query, request.PagingParams.PageNumber, request.PagingParams.PageSize);
        }
    }
}