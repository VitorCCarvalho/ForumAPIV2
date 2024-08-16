using AutoMapper;
using ForumCRUD.API.Data.Dtos.User;
using ForumCRUD.API.Models;

namespace ForumCRUD.API.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserDto, User>();
        CreateMap<User, ReadUserDto>();
    }
}
