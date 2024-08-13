using System.ComponentModel.DataAnnotations;

namespace ForumCRUD.API.Data.Dtos.Post;

public class CreatePostDto
{
    [Required(ErrorMessage = "Thread Id is required.")]
    public int ThreadId { get; set; }
    [Required(ErrorMessage = "Text is required.")]
    [StringLength(50, ErrorMessage = "Text need to have between 3 and 50 characters.")]
    public string Text { get; set; }
    [Required(ErrorMessage = "User Id is required.")]
    public string UserId { get; set; }
}
