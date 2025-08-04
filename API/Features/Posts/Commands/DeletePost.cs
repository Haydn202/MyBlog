using API.Data;
using AutoMapper;
using MediatR;

namespace API.Features.Posts.Commands;

public class DeletePost(Guid postId) : IRequest<bool>
{
    private Guid PostId { get; } = postId;
    
    private sealed class DeletePostHandler(
        DataContext dbContext) : IRequestHandler<DeletePost, bool>
    {
        public async Task<bool> Handle(DeletePost request, CancellationToken cancellationToken)
        {
            var post = await dbContext.Posts.FindAsync(request.PostId, cancellationToken);

            if (post == null)
            {
                return false;
            }
            
            dbContext.Posts.Remove(post);
            await dbContext.SaveChangesAsync(cancellationToken);
            
            return true;
        }
    }
}