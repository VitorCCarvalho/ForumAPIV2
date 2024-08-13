using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace ForumCRUD.API.Models;

public class FThread
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int ForumID { get; set; }

    public virtual Forum Forum { get; set; }
    [Required(ErrorMessage = "Name is required.")]
    [Range(3, 50, ErrorMessage = "Name need to have between 3 and 50 characters.")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Text is required.")]
    [Range(3, 2000, ErrorMessage = "Text need to have between 3 and 2000 characters.")]
    public string Text { get; set; }
    public bool? Sticky { get; set; } = false;
    public bool? Active { get; set; } = true;
    public DateTime DateCreated { get; set; } = DateTime.Now;
    public virtual User User { get; set; }
    public string UserId { get; set; }
    public bool? Locked { get; set; } = false;
    public virtual ICollection<Post> Posts { get; set; }
    public virtual ICollection<FThreadReaction> Reactions { get; set; }
}
