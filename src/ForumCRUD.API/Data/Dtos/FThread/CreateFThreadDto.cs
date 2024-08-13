using System.ComponentModel.DataAnnotations;

namespace ForumCRUD.API.Data.Dtos.FThread;

public class CreateFThreadDto
{
    [Required(ErrorMessage = "Forum Id is required.")]
    public int ForumID { get; set; }
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Name need to have between 3 and 50 characters.")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Text is required.")]
    [StringLength(2000, MinimumLength = 3, ErrorMessage = "Text need to have between 3 and 2000 characters.")]
    public string Text { get; set; }
    [Required(ErrorMessage = "StartedByUserId is required.")]
    public string UserId { get; set; }
}
