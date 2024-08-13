using System.ComponentModel.DataAnnotations;

namespace ForumCRUD.API.Data.Dtos.FThread;

public class UpdateFThreadDto
{
    [Required(ErrorMessage = "Forum Id is required.")]
    public int ForumID { get; set; }
    [Required(ErrorMessage = "Name is required.")]
    [Range(3, 50, ErrorMessage = "Name need to have between 3 and 50 characters.")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Text is required.")]
    [Range(3, 2000, ErrorMessage = "Text need to have between 3 and 2000 characters.")]
    public string Text { get; set; }
    [Required(ErrorMessage = "StartedByUserId is required.")]
    public string UserId { get; set; }
}
