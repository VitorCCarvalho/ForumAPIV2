using ForumCRUD.API.Data.Dtos.User;
using ForumCRUD.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ForumCRUD.API.Services;

public class TokenService
{
    private IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> GenerateToken(User user)
    {
        Claim[] claims = new Claim[]
        {
            new Claim("username", user.UserName),
            new Claim("name", user.Name),
            new Claim("id", user.Id),
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("98ahyd9asn8dhFN0SDFNASDLKFNDSF0SD8uyr9esj8fdso0i"));

        var signingCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken
            (
            expires: DateTime.Now.AddMinutes(10),
            claims: claims,
            signingCredentials: signingCredentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public async Task<string> GenerateToken(LoginUserDto loginDto)
    {
        // This version is for when we have a login DTO but not yet a full user object
        // In a real implementation, you'd want to retrieve the user from the database here
        Claim[] claims = new Claim[]
        {
            new Claim("username", loginDto.Username),
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("98ahyd9asn8dhFN0SDFNASDLKFNDSF0SD8uyr9esj8fdso0i"));

        var signingCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken
            (
            expires: DateTime.Now.AddMinutes(10),
            claims: claims,
            signingCredentials: signingCredentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public async Task<string> GenerateToken(CreateUserDto createDto)
    {
        // This version is for when we have a create DTO but not yet a full user object
        // In a real implementation, you'd want to retrieve the user from the database here
        Claim[] claims = new Claim[]
        {
            new Claim("username", createDto.Username),
            new Claim("name", createDto.Name),
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("98ahyd9asn8dhFN0SDFNASDLKFNDSF0SD8uyr9esj8fdso0i"));

        var signingCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken
            (
            expires: DateTime.Now.AddMinutes(10),
            claims: claims,
            signingCredentials: signingCredentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
