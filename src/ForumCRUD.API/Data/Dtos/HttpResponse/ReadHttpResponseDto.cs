using System.ComponentModel.DataAnnotations;

namespace ForumCRUD.API.Data.Dtos.HttpResponse;

public class ReadHttpResponseDto
{
    [Key]
    public string response { get; set; }
}
