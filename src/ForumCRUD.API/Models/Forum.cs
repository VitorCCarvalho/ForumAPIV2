using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace ForumCRUD.API.Models;

public class Forum
{
    [Key]
    [Required]
    public int Id { get; set; }
    [Required] public string Name { get; set; }
    [Required] public string Description { get; set; }
    public virtual ICollection<FThread> Threads { get; set; }

}
