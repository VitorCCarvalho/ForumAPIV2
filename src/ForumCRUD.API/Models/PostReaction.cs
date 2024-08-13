using System.ComponentModel.DataAnnotations;

namespace ForumCRUD.API.Models;

public class PostReaction
{
    [Required(ErrorMessage = "Name is required.")]
    public int PostId { get; set; }
    [Required(ErrorMessage = "UserId is required.")]
    public string UserId { get; set; }
    [Required(ErrorMessage = "Reaction is required.")]
    public bool Reaction { get; set; }

    public virtual Post Post { get; set; }
    public virtual User User { get; set; }
}
