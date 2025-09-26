using API.DTOs.Topics;
using API.Entities;
using API.Features.Topics.Commands;
using AutoMapper;

namespace API.Profiles;

public class TopicProfile : Profile
{
    public TopicProfile()
    {
        CreateMap<Topic, TopicDto>();
        CreateMap<TopicCreateDto, Topic>();
        CreateMap<TopicUpdateDto, Topic>();
        CreateMap<TopicCreateDto, CreateTopicCommandRequest>();
        CreateMap<TopicUpdateDto, UpdateTopicCommandRequest>();
        CreateMap<(TopicUpdateDto Dto, Guid Id), UpdateTopicCommandRequest>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Dto.Name))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Dto.Color));
    }
}