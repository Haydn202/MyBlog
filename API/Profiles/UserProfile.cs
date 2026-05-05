using API.DTOs.Accounts;
using API.Entities;
using AutoMapper;

namespace API.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.Token, o => o.MapFrom(_ => string.Empty))
            .ForMember(d => d.RefreshToken, o => o.Ignore());
    }
}