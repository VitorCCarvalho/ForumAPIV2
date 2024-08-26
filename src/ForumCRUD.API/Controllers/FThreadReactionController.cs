using AutoMapper;
using ForumCRUD.API.Data.Dtos.FThread;
using ForumCRUD.API.Data.Dtos.FThreadReaction;
using ForumCRUD.API.Data;
using ForumCRUD.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ForumCRUD.API.Controllers;

[ApiController]
[Route("[controller]")]
public class FThreadReactionController : ControllerBase
{
    private ForumContext _context;
    private IMapper _mapper;

    public FThreadReactionController(ForumContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]
    public IActionResult PostFThreadReaction([FromBody] CreateFThreadReactionDto dto)
    {
        FThreadReaction fThreadReaction = _mapper.Map<FThreadReaction>(dto);
        _context.Add(fThreadReaction);
        _context.SaveChanges();
        return Created("FThread Reaction Created", fThreadReaction);
    }

    [HttpGet]
    public IEnumerable<ReadFThreadReactionDto> GetFThreadReactions([FromQuery] int take = 50)
    {

        return _mapper.Map<List<ReadFThreadReactionDto>>(_context.fthreadreaction.Take(take).ToList());
    }

    [HttpGet("{fthreadId}")]
    public IEnumerable<ReadFThreadReactionDto> GetFThreadReactionPorFThread(int fthreadId, [FromQuery] string? reaction)
    {
        if (reaction == "like")
        {
            return _mapper.Map<List<ReadFThreadReactionDto>>(_context.fthreadreaction.Where(fthreadReaction => fthreadReaction.ThreadId == fthreadId & fthreadReaction.Reaction == true).ToList());
        }
        else if (reaction == "dislike")
        {
            return _mapper.Map<List<ReadFThreadReactionDto>>(_context.fthreadreaction.Where(fthreadReaction => fthreadReaction.ThreadId == fthreadId & fthreadReaction.Reaction == false).ToList());
        }
        else
        {
            return _mapper.Map<List<ReadFThreadReactionDto>>(_context.fthreadreaction.Where(fthreadReaction => fthreadReaction.ThreadId == fthreadId).ToList());
        }
    }

    [Route("score/{fthreadId}")]
    [HttpGet]
    public int GetFThreadReactionScore(int fthreadId)
    {
        int dislikes = _context.fthreadreaction.Where(fthreadReaction => fthreadReaction.ThreadId == fthreadId && !fthreadReaction.Reaction).Count();

        int likes = _context.fthreadreaction.Where(fthreadReaction => fthreadReaction.ThreadId == fthreadId & fthreadReaction.Reaction == true).Count();

        return likes - dislikes;
    }


    [HttpGet("{fthreadId}/{userId}")]
    public ReadFThreadReactionDto GetFThreadReaction(int fthreadId, string UserId)
    {
        return _mapper.Map<ReadFThreadReactionDto>(_context.fthreadreaction.FirstOrDefault(fthreadReaction => fthreadReaction.ThreadId == fthreadId & fthreadReaction.UserId == UserId));
    }

    [HttpPut("{fthreadId}/{userId}")]
    public IActionResult PutFThreadReaction([FromQuery] int fThreadId, [FromQuery] string UserId, [FromBody] UpdateFThreadDto dto)
    {
        var fthreadReaction = _context.fthreadreaction.FirstOrDefault(fthreadReaction => fthreadReaction.ThreadId == fThreadId & fthreadReaction.UserId == UserId);
        if (fthreadReaction == null)
        {
            return NotFound();
        }
        _mapper.Map(dto, fthreadReaction);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{fthreadId}/{userId}")]
    public IActionResult DeleteFThread(int fthreadId, string UserId)
    {
        var fthreadReaction = _context.fthreadreaction.FirstOrDefault(fthreadReaction => fthreadReaction.ThreadId == fthreadId & fthreadReaction.UserId == UserId);
        if (fthreadReaction == null)
        {
            return NotFound();
        }
        _context.Remove(fthreadReaction);
        _context.SaveChanges();
        return NoContent();
    }
}
