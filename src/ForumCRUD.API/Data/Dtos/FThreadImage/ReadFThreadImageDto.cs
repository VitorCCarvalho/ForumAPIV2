using System.ComponentModel.DataAnnotations;

namespace ForumCRUD.API.Data.Dtos.FThreadImage;

public class ReadFThreadImageDto
{
    public int Id { get; set; }

    [Required]
    public int FThreadId { get; set; }

    [Required]
    public string ImgId { get; set; }
}
