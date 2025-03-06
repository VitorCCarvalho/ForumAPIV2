using AutoMapper;
using ForumCRUD.API.Data.Dtos.FThread;
using ForumCRUD.API.Data.Dtos.FThreadReaction;
using ForumCRUD.API.Data;
using ForumCRUD.API.Models;
using ForumCRUD.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ForumCRUD.API.Controllers;

[ApiController]
[Route("[controller]")]
public class FThreadReactionController : ControllerBase
{
    private ForumContext _context;
    private IMapper _mapper;
    private readonly DatabaseQueueService _queueService;

    public FThreadReactionController(ForumContext context, IMapper mapper, DatabaseQueueService queueService)
    {
        _context = context;
        _mapper = mapper;
        _queueService = queueService;
    }

    [HttpPost]
    public async Task<IActionResult> PostFThreadReaction([FromBody] CreateFThreadReactionDto dto)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            FThreadReaction fThreadReaction = _mapper.Map<FThreadReaction>(dto);
            _context.Add(fThreadReaction);
            await _context.SaveChangesAsync();
            return Created("FThread Reaction Created", fThreadReaction);
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetFThreadReactions([FromQuery] int take = 50)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            return Ok(_mapper.Map<List<ReadFThreadReactionDto>>(await _context.fthreadreaction.Take(take).ToListAsync()));
        });
    }

    [HttpGet("{fthreadId}")]
    public async Task<IActionResult> GetFThreadReactionPorFThread(int fthreadId, [FromQuery] string? reaction)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            if (reaction == "like")
            {
                return Ok(_mapper.Map<List<ReadFThreadReactionDto>>(await _context.fthreadreaction.Where(fthreadReaction => fthreadReaction.ThreadId == fthreadId & fthreadReaction.Reaction == true).ToListAsync()));
            }
            else if (reaction == "dislike")
            {
                return Ok(_mapper.Map<List<ReadFThreadReactionDto>>(await _context.fthreadreaction.Where(fthreadReaction => fthreadReaction.ThreadId == fthreadId & fthreadReaction.Reaction == false).ToListAsync()));
            }
            else
            {
                return Ok(_mapper.Map<List<ReadFThreadReactionDto>>(await _context.fthreadreaction.Where(fthreadReaction => fthreadReaction.ThreadId == fthreadId).ToListAsync()));
            }
        });
    }

    [Route("score/{fthreadId}")]
    [HttpGet]
    public async Task<IActionResult> GetFThreadReactionScore(int fthreadId)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            int dislikes = await _context.fthreadreaction.Where(fthreadReaction => fthreadReaction.ThreadId == fthreadId && !fthreadReaction.Reaction).CountAsync();

            int likes = await _context.fthreadreaction.Where(fthreadReaction => fthreadReaction.ThreadId == fthreadId & fthreadReaction.Reaction == true).CountAsync();

            return Ok(likes - dislikes);
        });
    }

    [HttpGet("{fthreadId}/{userId}")]
    public async Task<IActionResult> GetFThreadReaction(int fthreadId, string UserId)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            return Ok(_mapper.Map<ReadFThreadReactionDto>(await _context.fthreadreaction.FirstOrDefaultAsync(fthreadReaction => fthreadReaction.ThreadId == fthreadId & fthreadReaction.UserId == UserId)));
        });
    }

    [HttpPut("{fthreadId}/{userId}")]
    public async Task<IActionResult> PutFThreadReaction([FromQuery] int fThreadId, [FromQuery] string UserId, [FromBody] UpdateFThreadDto dto)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            var fthreadReaction = await _context.fthreadreaction.FirstOrDefaultAsync(fthreadReaction => fthreadReaction.ThreadId == fThreadId & fthreadReaction.UserId == UserId);
            if (fthreadReaction == null)
            {
                return NotFound();
            }
            _mapper.Map(dto, fthreadReaction);
            await _context.SaveChangesAsync();
            return NoContent();
        });
    }

    [HttpDelete("{fthreadId}/{userId}")]
    public async Task<IActionResult> DeleteFThread(int fthreadId, string UserId)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            var fthreadReaction = await _context.fthreadreaction.FirstOrDefaultAsync(fthreadReaction => fthreadReaction.ThreadId == fthreadId & fthreadReaction.UserId == UserId);
            if (fthreadReaction == null)
            {
                return NotFound();
            }
            _context.Remove(fthreadReaction);
            await _context.SaveChangesAsync();
            return NoContent();
        });
    }
}
