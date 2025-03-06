using AutoMapper;
using ForumCRUD.API.Data.Dtos.User;
using ForumCRUD.API.Models;
using Microsoft.AspNetCore.Identity;

namespace ForumCRUD.API.Services;

public class UserService
{
    private IMapper _mapper;
    private UserManager<User> _userManager;
    private SignInManager<User> _signInManager;
    private TokenService _tokenService;
    private DatabaseQueueService _queueService;

    public UserService(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, TokenService tokenService, DatabaseQueueService queueService)
    {
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _queueService = queueService;
    }

    public async Task<string> SignUp(CreateUserDto dto)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync(async () =>
        {
            User user = _mapper.Map<User>(dto);

            IdentityResult resultado = await _userManager.CreateAsync(user, dto.Password);

            if (!resultado.Succeeded)
            {
                return "error";
            }

            return await _tokenService.GenerateToken(user);
        });
    }

    internal async Task<string> Login(LoginUserDto dto)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync(async () =>
        {
            var resultado = await _signInManager.PasswordSignInAsync(dto.Username, dto.Password, false, false);

            if (!resultado.Succeeded)
            {
                return "unauthorized";
            }

            var user = _signInManager.UserManager.Users.FirstOrDefault(user => user.NormalizedUserName == dto.Username.ToUpper());

            return await _tokenService.GenerateToken(user);
        });
    }

    public async Task<ReadUserDto> GetUser(string userId)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync(async () =>
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userMap = _mapper.Map<ReadUserDto>(user);
                return userMap;
            }
            else
            {
                throw new ApplicationException("Usuário não encontrado");
            }
        });
    }
}
