using API.Data;
using API.DTOs.Topics;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Topics.Queries;

public class GetTopics : IRequest<List<TopicDto>>
{
    private sealed class GetTopicsHandler(DataContext dbContext, IMapper mapper) : IRequestHandler<GetTopics, List<TopicDto>>
    {
        public async Task<List<TopicDto>> Handle(GetTopics request, CancellationToken cancellationToken)
        {
            var topics = await dbContext.Topics.ToListAsync(cancellationToken: cancellationToken);

            return mapper.Map<List<TopicDto>>(topics);
        }
    }
}