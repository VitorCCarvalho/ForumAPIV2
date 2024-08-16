using AutoMapper;
using ForumCRUD.API.Data.Dtos.FThreadReaction;
using ForumCRUD.API.Models;

namespace ForumCRUD.API.Profiles;

public class FThreadReactionProfile : Profile
{
    public FThreadReactionProfile()
    {
        CreateMap<CreateFThreadReactionDto, FThreadReaction>();
        CreateMap<FThreadReaction, ReadFThreadReactionDto>();
        CreateMap<UpdateFThreadReactionDto, FThreadReaction>();
        CreateMap<FThreadReaction, UpdateFThreadReactionDto>();
    }
}
