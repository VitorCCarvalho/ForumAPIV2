using AutoMapper;
using ForumCRUD.API.Data.Dtos.PostReaction;
using ForumCRUD.API.Models;

namespace ForumCRUD.API.Profiles;

public class PostReactionProfile : Profile
{
    public PostReactionProfile()
    {
        CreateMap<CreatePostReactionDto, PostReaction>();
        CreateMap<PostReaction, ReadPostReactionDto>();
        CreateMap<UpdatePostReactionDto, PostReaction>();
        CreateMap<PostReaction, UpdatePostReactionDto>();
    }
}
