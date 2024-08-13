using System.ComponentModel.DataAnnotations;

namespace ForumCRUD.API.Models;

public class FThreadReaction
{
    [Required(ErrorMessage = "Name is required.")]
    public int ThreadId { get; set; }
    [Required(ErrorMessage = "UserId is required.")]
    public string UserId { get; set; }
    [Required(ErrorMessage = "Reaction is required.")]
    public bool Reaction { get; set; }

    public virtual FThread FThread { get; set; }
    public virtual User User { get; set; }
}
