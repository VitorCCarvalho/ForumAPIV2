using System.ComponentModel.DataAnnotations;

namespace ForumCRUD.API.Data.Dtos.User;

public class CreateUserDto
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string RePassword { get; set; }
}
