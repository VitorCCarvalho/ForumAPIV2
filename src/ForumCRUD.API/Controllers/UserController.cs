using ForumCRUD.API.Data.Dtos.HttpResponse;
using ForumCRUD.API.Data.Dtos.User;
using ForumCRUD.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ForumCRUD.API.Controllers;

[ApiController]
[Route("[Controller]")]
public class UserController : ControllerBase
{
    private UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Cadastra um User novo.
    /// </summary>
    [HttpPost("signup")]
    public async Task<IActionResult> SignUpUser(CreateUserDto dto)
    {
        var result = await _userService.SignUp(dto);
        if (result == "error")
        {
            return BadRequest(dto);
        }
        else
        {
            var responseDto = new ReadHttpResponseDto();
            responseDto.response = result;
            return Ok(responseDto);
        }

    }

    /// <summary>
    /// Verifica se o user existe e retorna token de sessão jwt.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(LoginUserDto dto)
    {
        var token = await _userService.Login(dto);

        if (token == "unauthorized")
        {
            return Unauthorized("Usuário ou senha incorretos");
        }
        else
        {
            var responseDto = new ReadHttpResponseDto();
            responseDto.response = token;
            return Ok(responseDto);
        }

    }

    /// <summary>
    /// Retorna todos Users.
    /// </summary>
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(string userId)
    {
        var user = await _userService.GetUser(userId);
        return Ok(user);
    }
}
