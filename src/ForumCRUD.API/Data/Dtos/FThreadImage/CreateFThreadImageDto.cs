using System.ComponentModel.DataAnnotations;

namespace ForumCRUD.API.Data.Dtos.FThreadImage;

public class CreateFThreadImageDto
{
    [Required(ErrorMessage = "FThread Id is required.")]
    public int FThreadId { get; set; }
    [Required(ErrorMessage = "Img Id is required.")]
    public string ImgId { get; set; }

}
