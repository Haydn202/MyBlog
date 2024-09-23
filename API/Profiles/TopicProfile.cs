using API.DTOs.Topics;
using API.Entities;
using AutoMapper;

namespace API.Profiles;

public class TopicProfile : Profile
{
    public TopicProfile()
    {
        CreateMap<Topic, TopicDto>();
        CreateMap<TopicCreateDto, Topic>();
        CreateMap<TopicUpdateDto, Topic>();
    }
}