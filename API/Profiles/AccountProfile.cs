using API.DTOs.Accounts;
using API.Features.Accounts.Commands;
using AutoMapper;

namespace API.Profiles;

public class AccountProfile : Profile
{
    public AccountProfile()
    {
        CreateMap<RegisterDto, RegisterUserCommandRequest>();
        CreateMap<LoginDto, LoginCommandRequest>();
    }
} 