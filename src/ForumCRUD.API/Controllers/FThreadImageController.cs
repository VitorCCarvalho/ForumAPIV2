using AutoMapper;
using ForumCRUD.API.Data;
using ForumCRUD.API.Data.Dtos.FThread;
using ForumCRUD.API.Data.Dtos.FThreadImage;
using ForumCRUD.API.Models;
using ForumCRUD.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ForumCRUD.API.Controllers;

[ApiController]
[Route("[controller]")]
public class FThreadImageController : ControllerBase
{
    private ForumContext _context;
    private IMapper _mapper;
    private readonly DatabaseQueueService _queueService;

    public FThreadImageController(ForumContext context, IMapper mapper, DatabaseQueueService queueService)
    {
        _context = context;
        _mapper = mapper;
        _queueService = queueService;
    }

    [HttpPost]
    public async Task<IActionResult> PostFThreadImage([FromBody] CreateFThreadImageDto dto)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            FThreadImage fthreadImage = _mapper.Map<FThreadImage>(dto);
            _context.Add(fthreadImage);
            await _context.SaveChangesAsync();
            return Created("ThreadImage created", fthreadImage);
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetFThreadImages([FromQuery] int? fthreadId, [FromQuery] int take = 50)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            if (fthreadId == null)
            {
                return Ok(_mapper.Map<List<ReadFThreadImageDto>>(await _context.fthreadimage.Take(take).ToListAsync()));
            }
            else
            {
                return Ok(_mapper.Map<List<ReadFThreadImageDto>>(await _context.fthreadimage.Take(take).
                                                            Where(fthreadImage => fthreadImage.FThreadId == fthreadId).ToListAsync()));
            }
        });
    }

    [HttpPut("{fthreadImageId}")]
    public async Task<IActionResult> PutFThreadImage(int fthreadImageId, [FromBody] UpdateFThreadImageDto dto)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            var fthreadImage = await _context.fthreadimage.FirstOrDefaultAsync(fthreadImage => fthreadImage.Id == fthreadImageId);
            if (fthreadImage == null)
            {
                return NotFound();
            }
            _mapper.Map(dto, fthreadImage);
            await _context.SaveChangesAsync();
            return NoContent();
        });
    }

    [HttpDelete("{fthreadImageId}")]
    public async Task<IActionResult> DeleteFThreadImage(int fthreadImageId)
    {
        return await _queueService.ExecuteWithConnectionLimitAsync<IActionResult>(async () =>
        {
            var fthread = await _context.threads.FirstOrDefaultAsync(fthread => fthread.Id == fthreadImageId);
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
