using AutoMapper;
using ForumCRUD.API.Data.Dtos.Forum;
using ForumCRUD.API.Data;
using ForumCRUD.API.Models;
using ForumCRUD.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ForumApp.Controllers;

[ApiController]
[Route("[controller]")]
public class ForumController : ControllerBase
{
    private ForumContext _context;
    private IMapper _mapper;
    private readonly DatabaseQueueService _queueService;

    public ForumController(ForumContext forumContext, IMapper mapper, DatabaseQueueService queueService)
    {
        _context = forumContext;
        _mapper = mapper;
        _queueService = queueService;
    }

    /// <summary>
    /// Cria um fórum novo.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> PostForum([FromBody] CreateForumDto dto)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            Forum forum = _mapper.Map<Forum>(dto);
            _context.Add(forum);
            await _context.SaveChangesAsync();
            return Created("Forum created", forum);
        });
    }

    /// <summary>
    /// Retorna todos os fóruns.
    /// </summary>
    [HttpGet]
    public async Task<IEnumerable<ReadForumDto>> GetForums([FromQuery] int take = 50, [FromQuery] int? forumId = null)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync(async () =>
        {
            if (forumId == null)
            {
                return _mapper.Map<List<ReadForumDto>>(await _context.forums.Take(take).ToListAsync());
            }
            else
            {
                return _mapper.Map<List<ReadForumDto>>(await _context.forums.Take(take)
                                                       .Where(forum => forum.Id == forumId).ToListAsync());
            }
        });
    }

    /// <summary>
    /// Retorna o fórum que possui forumId como ID.
    /// </summary>
    [HttpGet("{forumId}")]
    public async Task<ReadForumDto> GetForumsById(int forumId, [FromQuery] int take = 50)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync(async () =>
        {
            return _mapper.Map<ReadForumDto>(await _context.forums.FirstOrDefaultAsync(forum => forum.Id == forumId));
        });
    }

    /// <summary>
    /// Atualiza o fórum que possui forumId como ID.
    /// </summary>
    [HttpPut("{forumId}")]
    public async Task<IActionResult> PutForum(int forumId, [FromBody] UpdateForumDto dto)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            var forum = await _context.forums.FirstOrDefaultAsync(forum => forum.Id == forumId);
            if (forum == null)
            {
                return NotFound();
            }
            _mapper.Map(dto, forum);
            await _context.SaveChangesAsync();
            return NoContent();
        });
    }

    /// <summary>
    /// Atualiza uma parte do fórum que possui forumId como ID.
    /// </summary>
    [HttpPatch("{forumId}")]
    public async Task<IActionResult> PatchForum(int forumId, JsonPatchDocument<UpdateForumDto> patch)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            var forum = await _context.forums.FirstOrDefaultAsync(forum => forum.Id == forumId);
            if (forum == null)
            {
                return NotFound();
            }
            var forumPatch = _mapper.Map<UpdateForumDto>(forum);
            patch.ApplyTo(forumPatch, ModelState);

            if (!TryValidateModel(forumPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(forumPatch, forum);
            await _context.SaveChangesAsync();
            return NoContent();
        });
    }

    /// <summary>
    /// Deleta o fórum que possui forumId como ID.
    /// </summary>
    [HttpDelete("{forumId}")]
    public async Task<IActionResult> DeleteForum(int forumId)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            var forum = await _context.forums.FirstOrDefaultAsync(forum => forum.Id == forumId);
            if (forum == null)
            {
                return NotFound();
            }
            _context.Remove(forum);
            await _context.SaveChangesAsync();
            return NoContent();
        });
    }
}
