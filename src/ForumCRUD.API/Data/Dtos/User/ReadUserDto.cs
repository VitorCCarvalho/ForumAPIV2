namespace ForumCRUD.API.Data.Dtos.User;

using ForumCRUD.API.Data.Dtos.FThread;
using ForumCRUD.API.Data.Dtos.Post;
public class ReadUserDto
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime LastLogin { get; set; }
    public DateTime DateJoined { get; set; } = DateTime.Now;
    public virtual ICollection<ReadFThreadDto> Threads { get; set; }
    public virtual ICollection<ReadPostDto> Posts { get; set; }
}
