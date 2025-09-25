using API.Data;
using API.DTOs.Posts;
using API.Entities;
using API.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Posts.Queries;

public class GetPosts(PagingParams pagingParams, PostFilters? filters = null) : IRequest<PaginatedResult<PostSummaryDto>>
{
    private PagingParams PagingParams { get; } = pagingParams;
    private PostFilters? Filters { get; } = filters;

    private sealed class GetPostsHandler(
        DataContext dbContext, 
        IMapper mapper) : IRequestHandler<GetPosts, PaginatedResult<PostSummaryDto>>
    {
        public async Task<PaginatedResult<PostSummaryDto>> Handle(GetPosts request, CancellationToken cancellationToken)
        {
            var query = dbContext.Posts
                .Include("Topics")
                .AsQueryable();

            if (request.Filters != null)
            {
                if (request.Filters.Status.HasValue)
                {
                    query = query.Where(p => p.Status == request.Filters.Status.Value);
                }

                if (!string.IsNullOrEmpty(request.Filters.TopicId))
                {
                    query = query.Where(p => p.Topics.Any(t => t.Id.ToString() == request.Filters.TopicId));
                }

                if (!string.IsNullOrEmpty(request.Filters.SearchTerm))
                {
                    var searchTerm = request.Filters.SearchTerm.ToLower();
                    query = query.Where(p => 
                        p.Title.ToLower().Contains(searchTerm) || 
                        p.Description.ToLower().Contains(searchTerm));
                }
            }

            var projectedQuery = query
                .OrderByDescending(x => x.CreatedOn)
                .ProjectTo<PostSummaryDto>(mapper.ConfigurationProvider);
            
            return await PaginationHelper.CreateAsync(projectedQuery, request.PagingParams.PageNumber, request.PagingParams.PageSize);
        }
    }
}

public class PostFilters
{
    public PostStatus? Status { get; set; }
    public string? TopicId { get; set; }
    public string? SearchTerm { get; set; }
}