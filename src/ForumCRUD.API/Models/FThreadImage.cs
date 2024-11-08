using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ForumCRUD.API.Models;

public class FThreadImage
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int FThreadId { get; set; }

    [Required]
    public string ImgId { get; set; }

    public virtual FThread FThread { get; set; }

}
