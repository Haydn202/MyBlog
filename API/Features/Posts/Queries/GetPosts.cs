using API.Data;
using API.DTOs.Posts;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Posts.Queries;

public class GetPosts() : IRequest<List<PostSummaryDto>>
{
    private sealed class GetPostsHandler(
        DataContext dbContext, 
        IMapper mapper) : IRequestHandler<GetPosts, List<PostSummaryDto>>
    {
        public async Task<List<PostSummaryDto>> Handle(GetPosts request, CancellationToken cancellationToken)
        {
            var posts = await dbContext.Posts
                .OrderByDescending(x => x.CreatedOn)
                .Take(10)
                .Include("Topics")
                .ToListAsync(cancellationToken: cancellationToken);

            return mapper.Map<List<PostSummaryDto>>(posts);
        }
    }
}