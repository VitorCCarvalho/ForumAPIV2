using AutoMapper;
using ForumCRUD.API.Data.Dtos.FThread;
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
public class FThreadController : ControllerBase
{
    private ForumContext _context;
    private IMapper _mapper;
    private readonly DatabaseQueueService _queueService;

    public FThreadController(ForumContext context, IMapper mapper, DatabaseQueueService queueService)
    {
        _context = context;
        _mapper = mapper;
        _queueService = queueService;
    }

    /// <summary>
    /// Cria uma thread nova.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> PostFThread([FromBody] CreateFThreadDto dto)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            FThread fthread = _mapper.Map<FThread>(dto);
            _context.Add(fthread);
            await _context.SaveChangesAsync();
            return Created("Thread created", fthread);
        });
    }

    /// <summary>
    /// Retorna todas as threads.
    /// </summary>
    [HttpGet]
    public async Task<IEnumerable<ReadFThreadDto>> GetFThreads([FromQuery] int? forumId, [FromQuery] int take = 50)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IEnumerable<ReadFThreadDto>>(async () =>
        {
            if (forumId == null)
            {
                return _mapper.Map<List<ReadFThreadDto>>(await _context.threads.Take(take).ToListAsync());
            }
            else
            {
                return _mapper.Map<List<ReadFThreadDto>>(await _context.threads.Take(take)
                                                        .Where(fthread => fthread.ForumID == forumId).ToListAsync());
            }
        });
    }

    [Route("most-liked/{period}")]
    [HttpGet]
    public async Task<IEnumerable<ReadFThreadDto>> GetFThreads(int period)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IEnumerable<ReadFThreadDto>>(async () =>
        {
            return _mapper.Map<List<ReadFThreadDto>>(await _context.threads.Where(fthread => fthread.DateCreated >= DateTime.Today.AddDays(-period)).ToListAsync());
        });
    }

    /// <summary>
    /// Retorna a thread que possui fthreadId como ID.
    /// </summary>
    [HttpGet("{fthreadId}")]
    public async Task<ReadFThreadDto> GetFThreadById(int fthreadId)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<ReadFThreadDto>(async () =>
        {
            return _mapper.Map<ReadFThreadDto>(await _context.threads.FirstOrDefaultAsync(fthread => fthread.Id == fthreadId));
        });
    }

    /// <summary>
    /// Atualiza a thread que possui fthreadId como ID.
    /// </summary>
    [HttpPut("{fthreadId}")]
    public async Task<IActionResult> PutFThread(int fthreadId, [FromBody] UpdateFThreadDto dto)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            var fthread = await _context.threads.FirstOrDefaultAsync(fthread => fthread.Id == fthreadId);
            if (fthread == null)
            {
                return NotFound();
            }
            _mapper.Map(dto, fthread);
            await _context.SaveChangesAsync();
            return NoContent();
        });
    }

    /// <summary>
    /// Atualiza uma parte da thread que possui fthreadId como ID.
    /// </summary>
    [HttpPatch("{fthreadId}")]
    public async Task<IActionResult> PatchFThread(int fthreadId, JsonPatchDocument<UpdateFThreadDto> patch)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            var fthread = await _context.threads.FirstOrDefaultAsync(fthread => fthread.Id == fthreadId);
            if (fthread == null)
            {
                return NotFound();
            }
            var fthreadPatch = _mapper.Map<UpdateFThreadDto>(fthread);
            patch.ApplyTo(fthreadPatch, ModelState);

            if (!TryValidateModel(fthreadPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(fthreadPatch, fthread);
            await _context.SaveChangesAsync();
            return NoContent();
        });
    }

    /// <summary>
    /// Deleta a thread que possui fthreadId como ID.
    /// </summary>
    [HttpDelete("{fthreadId}")]
    public async Task<IActionResult> DeleteFThread(int fthreadId)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            var fthread = await _context.threads.FirstOrDefaultAsync(fthread => fthread.Id == fthreadId);
            if (fthread == null)
            {
                return NotFound();
            }
            _context.Remove(fthread);
            await _context.SaveChangesAsync();
            return NoContent();
        });
    }
}
