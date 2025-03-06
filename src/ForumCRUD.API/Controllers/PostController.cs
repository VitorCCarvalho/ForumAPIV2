using AutoMapper;
using ForumCRUD.API.Data.Dtos.Forum;
using ForumCRUD.API.Data.Dtos.Post;
using ForumCRUD.API.Data;
using ForumCRUD.API.Models;
using ForumCRUD.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ForumCRUD.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    ForumContext _context;
    private IMapper _mapper;
    private readonly DatabaseQueueService _queueService;

    public PostController(ForumContext context, IMapper mapper, DatabaseQueueService queueService)
    {
        _context = context;
        _mapper = mapper;
        _queueService = queueService;
    }

    /// <summary>
    /// Cria um post novo.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> PostFPost([FromBody] CreatePostDto dto)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            Post post = _mapper.Map<Post>(dto);
            _context.Add(post);
            await _context.SaveChangesAsync();
            return Created("Post created", post);
        });
    }

    /// <summary>
    /// Retorna todos os posts.
    /// </summary>
    [HttpGet]
    public async Task<IEnumerable<ReadPostDto>> GetPosts([FromQuery] int? fthreadId, [FromQuery] int take = 50)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IEnumerable<ReadPostDto>>(async () =>
        {
            if (fthreadId == null)
            {
                return _mapper.Map<List<ReadPostDto>>(await _context.posts.Take(take).ToListAsync());
            }
            else
            {
                return _mapper.Map<List<ReadPostDto>>(await _context.posts.Take(take).
                                                        Where(post => post.ThreadId == fthreadId).ToListAsync());
            }
        });
    }

    /// <summary>
    /// Retorna o post que possui postId como ID.
    /// </summary>
    [HttpGet("{postId}")]
    public async Task<ReadPostDto> GetPostById(int postId)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<ReadPostDto>(async () =>
        {
            return _mapper.Map<ReadPostDto>(await _context.posts.FirstOrDefaultAsync(post => post.Id == postId));
        });
    }

    /// <summary>
    /// Atualiza o post que possui postId como ID.
    /// </summary>
    [HttpPut("{postId}")]
    public async Task<IActionResult> PutPost(int postId, [FromBody] UpdatePostDto dto)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            var post = await _context.posts.FirstOrDefaultAsync(post => post.Id == postId);
            if (post == null)
            {
                return NotFound();
            }
            _mapper.Map(dto, post);
            await _context.SaveChangesAsync();
            return NoContent();
        });
    }

    /// <summary>
    /// Atualiza uma parte do post que possui postId como ID.
    /// </summary>
    [HttpPatch("{postId}")]
    public async Task<IActionResult> PatchPost(int postId, JsonPatchDocument<UpdateForumDto> patch)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            var post = await _context.posts.FirstOrDefaultAsync(post => post.Id == postId);
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
            await _context.SaveChangesAsync();
            return NoContent();
        });
    }

    /// <summary>
    /// Deleta o post que possui postId como ID.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int postId)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            var post = await _context.posts.FirstOrDefaultAsync(post => post.Id == postId);
            if (post == null)
            {
                return NotFound();
            }
            _context.Remove(post);
            await _context.SaveChangesAsync();
            return NoContent();
        });
    }
}
