using AutoMapper;
using ForumCRUD.API.Data.Dtos.Post;
using ForumCRUD.API.Models;

namespace ForumCRUD.API.Profiles;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<CreatePostDto, Post>();
        CreateMap<Post, ReadPostDto>();
        CreateMap<UpdatePostDto, Post>();
        CreateMap<Post, UpdatePostDto>();
    }
}
