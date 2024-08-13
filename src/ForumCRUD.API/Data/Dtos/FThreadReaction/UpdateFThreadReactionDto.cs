using System.ComponentModel.DataAnnotations;

namespace ForumCRUD.API.Data.Dtos.FThreadReaction;

public class UpdateFThreadReactionDto
{
    [Key]
    [Required(ErrorMessage = "Name is required.")]
    public int ThreadId { get; set; }
    [Key]
    [Required(ErrorMessage = "UserId is required.")]
    public string UserId { get; set; }
    [Required(ErrorMessage = "Reaction is required.")]
    public bool Reaction { get; set; }
}
