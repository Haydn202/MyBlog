using API.Data;
using API.DTOs.Posts;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Posts.Queries;

public class GetPost(Guid id) : IRequest<PostDto>
{
    private Guid Id { get; } = id;
    
    private sealed class GetPostHandler(DataContext dbContext, IMapper mapper) : IRequestHandler<GetPost, PostDto?>
    {
        public async Task<PostDto?> Handle(GetPost request, CancellationToken cancellationToken)
        {
            var post = await dbContext.Posts.Include("Topics").FirstOrDefaultAsync(x => 
                x.Id == request.Id, cancellationToken: cancellationToken);

            if (post is null)
            {
                return null;
            }

            return mapper.Map<PostDto>(post);
        }
    }
}