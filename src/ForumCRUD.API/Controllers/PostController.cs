using AutoMapper;
using ForumCRUD.API.Data.Dtos.Forum;
using ForumCRUD.API.Data.Dtos.Post;
using ForumCRUD.API.Data;
using ForumCRUD.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace ForumCRUD.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    ForumContext _context;
    private IMapper _mapper;

    public PostController(ForumContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Cria um post novo.
    /// </summary>
    [HttpPost]
    public IActionResult PostFPost([FromBody] CreatePostDto dto)
    {
        Post post = _mapper.Map<Post>(dto);
        _context.Add(post);
        _context.SaveChanges();
        return Created("Post created", post);
    }

    /// <summary>
    /// Retorna todos os posts.
    /// </summary>
    [HttpGet]
    public IEnumerable<ReadPostDto> GetPosts([FromQuery] int? fthreadId, [FromQuery] int take = 50)
    {
        if (fthreadId == null)
        {
            return _mapper.Map<List<ReadPostDto>>(_context.Posts.Take(take).ToList());
        }
        else
        {
            return _mapper.Map<List<ReadPostDto>>(_context.Posts.Take(take).
                                                    Where(post => post.ThreadId == fthreadId));
        }
    }

    /// <summary>
    /// Retorna o post que possui postId como ID.
    /// </summary>
    [HttpGet("{postId}")]
    public ReadPostDto GetPostById(int postId)
    {
        return _mapper.Map<ReadPostDto>(_context.Posts.FirstOrDefault(post => post.Id == postId));
    }

    /// <summary>
    /// Atualiza o post que possui postId como ID.
    /// </summary>
    [HttpPut("{postId}")]
    public IActionResult PutPost(int postId, [FromBody] UpdatePostDto dto)
    {
        var post = _context.Posts.FirstOrDefault(post => post.Id == postId);
        if (post == null)
        {
            return NotFound();
        }
        _mapper.Map(dto, post);
        _context.SaveChanges();
        return NoContent();
    }

    /// <summary>
    /// Atualiza uma parte do post que possui postId como ID.
    /// </summary>
    [HttpPatch("{postId}")]
    public IActionResult PatchForum(int postId, JsonPatchDocument<UpdateForumDto> patch)
    {
        var post = _context.Forums.FirstOrDefault(post => post.Id == postId);
        if (post == null)
        {
            return NotFound();
        }

        var postPatch = _mapper.Map<UpdateForumDto>(post);
        patch.ApplyTo(postPatch, ModelState);

        if (!TryValidateModel(postPatch))
        {
            return ValidationProblem(ModelState);
        }

        _mapper.Map(postPatch, post);
        _context.SaveChanges();
        return NoContent();
    }

    /// <summary>
    /// Deleta o post que possui postId como ID.
    /// </summary>
    [HttpDelete("{id}")]
    public IActionResult DeletePost(int postId)
    {
        var post = _context.Forums.FirstOrDefault(post => post.Id == postId);
        if (post == null)
        {
            return NotFound();
        }
        _context.Remove(post);
        _context.SaveChanges();
        return NoContent();
    }
}
