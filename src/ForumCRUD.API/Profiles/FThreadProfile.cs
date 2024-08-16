using AutoMapper;
using ForumCRUD.API.Data.Dtos.FThread;
using ForumCRUD.API.Models;

namespace ForumCRUD.API.Profiles;

public class FThreadProfile : Profile
{
    public FThreadProfile()
    {
        CreateMap<CreateFThreadDto, FThread>();
        CreateMap<UpdateFThreadDto, FThread>();
        CreateMap<FThread, ReadFThreadDto>()
            .ForMember(fthreadDto => fthreadDto.Posts,
                opt => opt.MapFrom(fthread => fthread.Posts));
    }
}
