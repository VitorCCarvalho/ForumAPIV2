using AutoMapper;
using ForumCRUD.API.Data.Dtos.Post;
using ForumCRUD.API.Data.Dtos.PostReaction;
using ForumCRUD.API.Data;
using ForumCRUD.API.Models;
using ForumCRUD.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ForumCRUD.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PostReactionController : ControllerBase
{
    private ForumContext _context;
    private IMapper _mapper;
    private readonly DatabaseQueueService _queueService;

    public PostReactionController(ForumContext context, IMapper mapper, DatabaseQueueService queueService)
    {
        _context = context;
        _mapper = mapper;
        _queueService = queueService;
    }

    [HttpPost]
    public async Task<IActionResult> PostFPostReaction([FromBody] CreatePostReactionDto dto)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            PostReaction postReaction = _mapper.Map<PostReaction>(dto);
            _context.Add(postReaction);
            await _context.SaveChangesAsync();
            return Created("Post Reaction Created", postReaction);
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetPostReactions([FromQuery] int? postId, [FromQuery] int take = 50)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            if (postId == null)
            {
                return Ok(_mapper.Map<List<ReadPostReactionDto>>(await _context.postreaction.Take(take).ToListAsync()));
            }
            else
            {
                return Ok(_mapper.Map<List<ReadPostReactionDto>>(await _context.postreaction.Take(take).
                                                            Where(postReaction => postReaction.PostId == postId).ToListAsync()));
            }
        });
    }

    [HttpGet("{postId}")]
    public async Task<IActionResult> GetFThreadReactionPorFThread(int postId, [FromQuery] string? reaction)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            if (reaction == "like")
            {
                return Ok(_mapper.Map<List<ReadPostReactionDto>>(await _context.postreaction.Where(postReaction => postReaction.PostId == postId & postReaction.Reaction == true).ToListAsync()));
            }
            else if (reaction == "dislike")
            {
                return Ok(_mapper.Map<List<ReadPostReactionDto>>(await _context.postreaction.Where(postReaction => postReaction.PostId == postId & postReaction.Reaction == false).ToListAsync()));
            }
            else
            {
                return Ok(_mapper.Map<List<ReadPostReactionDto>>(await _context.postreaction.Where(postReaction => postReaction.PostId == postId).ToListAsync()));
            }
        });
    }

    [Route("score/{postId}")]
    [HttpGet]
    public async Task<IActionResult> GetPostReactionScore(int postId)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            int dislikes = await _context.postreaction.Where(postReaction => postReaction.PostId == postId && !postReaction.Reaction).CountAsync();

            int likes = await _context.postreaction.Where(postReaction => postReaction.PostId == postId & postReaction.Reaction == true).CountAsync();

            return Ok(likes - dislikes);
        });
    }

    [HttpGet("{postId}/{userId}")]
    public async Task<IActionResult> GetPostReaction(int postId, string UserId)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            return Ok(_mapper.Map<ReadPostReactionDto>(await _context.postreaction.FirstOrDefaultAsync(postReaction => postReaction.PostId == postId & postReaction.UserId == UserId)));
        });
    }

    [HttpPut("{fthreadId}/{userId}")]
    public async Task<IActionResult> PutPostReaction([FromQuery] int fthreadId, [FromQuery] string UserId, [FromBody] UpdatePostDto dto)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            var postReaction = await _context.postreaction.FirstOrDefaultAsync(postReaction => postReaction.PostId == fthreadId & postReaction.UserId == UserId);
            if (postReaction == null)
            {
                return NotFound();
            }
            _mapper.Map(dto, postReaction);
            await _context.SaveChangesAsync();
            return NoContent();
        });
    }

    [HttpDelete("{postId}/{userId}")]
    public async Task<IActionResult> DeleteFThread(int postId, string UserId)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            var postReaction = await _context.postreaction.FirstOrDefaultAsync(postReaction => postReaction.PostId == postId & postReaction.UserId == UserId);
            if (postReaction == null)
            {
                return NotFound();
            }
            _context.Remove(postReaction);
            await _context.SaveChangesAsync();
            return NoContent();
        });
    }
}
