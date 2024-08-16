using AutoMapper;
using ForumCRUD.API.Data.Dtos.Forum;
using ForumCRUD.API.Models;

namespace ForumCRUD.API.Profiles;

public class ForumProfile : Profile
{
    public ForumProfile()
    {
        CreateMap<CreateForumDto, Forum>();
        CreateMap<Forum, UpdateForumDto>();
        CreateMap<UpdateForumDto, Forum>();
        CreateMap<Forum, ReadForumDto>()
            .ForMember(forumdto => forumdto.Threads,
                opt => opt.MapFrom(forum => forum.Threads));
    }
}
