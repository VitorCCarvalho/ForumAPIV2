using AutoMapper;
using ForumCRUD.API.Data.Dtos.FThreadImage;
using ForumCRUD.API.Models;

namespace ForumCRUD.API.Profiles;

public class FThreadImageProfile: Profile
{
    public FThreadImageProfile() {
        CreateMap<CreateFThreadImageDto, FThreadImage>();
        CreateMap<FThreadImage, ReadFThreadImageDto>();
        CreateMap<UpdateFThreadImageDto, FThreadImage>();
        CreateMap<FThreadImage, UpdateFThreadImageDto>();

    }
}
