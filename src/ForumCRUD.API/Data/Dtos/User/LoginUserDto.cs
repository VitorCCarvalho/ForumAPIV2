using System.ComponentModel.DataAnnotations;

namespace ForumCRUD.API.Data.Dtos.User;

public class LoginUserDto
{
    [Required]
    public String Username { get; set; }
    [Required]
    public String Password { get; set; }
}
