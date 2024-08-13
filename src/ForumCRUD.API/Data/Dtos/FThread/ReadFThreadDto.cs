using ForumCRUD.API.Data.Dtos.Post;
using System.ComponentModel.DataAnnotations;

namespace ForumCRUD.API.Data.Dtos.FThread;

public class ReadFThreadDto
{
    public int Id { get; set; }
    [Required]
    public int ForumID { get; set; }
    [Required(ErrorMessage = "Name is required.")]
    [Range(3, 50, ErrorMessage = "Name need to have between 3 and 50 characters.")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Text is required.")]
    [Range(3, 2000, ErrorMessage = "Text need to have between 3 and 2000 characters.")]
    public string Text { get; set; }
    public bool? Sticky { get; set; }
    public bool? Active { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.Now;
    public string UserId { get; set; }
    public bool? Locked { get; set; } = false;
    public ICollection<ReadPostDto> Posts { get; set; }
}
