using API.DTOs.Comments;
using API.Entities;
using API.Entities.Comments;
using AutoMapper;

namespace API.Profiles;

public class CommentProfile: Profile
{
    public CommentProfile()
    {
        CreateMap<MainComment, MainCommentDto>();
        CreateMap<SubComment, SubCommentDto>();
    }
}