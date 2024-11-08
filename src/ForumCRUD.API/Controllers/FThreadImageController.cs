using AutoMapper;
using ForumCRUD.API.Data;
using ForumCRUD.API.Data.Dtos.FThread;
using ForumCRUD.API.Data.Dtos.FThreadImage;
using ForumCRUD.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ForumCRUD.API.Controllers;

[ApiController]
[Route("[controller]")]
public class FThreadImageController : ControllerBase
{
    private ForumContext _context;
    private IMapper _mapper;

    public FThreadImageController(ForumContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]
    public IActionResult PostFThreadImage([FromBody] CreateFThreadImageDto dto)
    {
        FThreadImage fthreadImage = _mapper.Map<FThreadImage>(dto);
        _context.Add(fthreadImage);
        _context.SaveChanges();
        return Created("ThreadImage created", fthreadImage);
    }

    [HttpGet]
    public IEnumerable<ReadFThreadImageDto> GetFThreadImages([FromQuery] int? fthreadId, [FromQuery] int take = 50)
    {
        if (fthreadId == null)
        {
            return _mapper.Map<List<ReadFThreadImageDto>>(_context.fthreadimage.Take(take).ToList());
        }
        else
        {
            return _mapper.Map<List<ReadFThreadImageDto>>(_context.fthreadimage.Take(take).
                                                        Where(fthreadImage => fthreadImage.FThreadId == fthreadId).ToList());
        }
    }

    [HttpPut("{fthreadImageId}")]
    public IActionResult PutFThreadImage(int fthreadImageId, [FromBody] UpdateFThreadImageDto dto)
    {
        var fthreadImage = _context.fthreadimage.FirstOrDefault(fthreadImage => fthreadImage.Id == fthreadImageId);
        if (fthreadImage == null)
        {
            return NotFound();
        }
        _mapper.Map(dto, fthreadImage);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{fthreadImageId}")]
    public IActionResult DeleteFThreadImage(int fthreadImageId)
    {
        var fthread = _context.threads.FirstOrDefault(fthread => fthread.Id == fthreadImageId);
        if (fthread == null)
        {
            return NotFound();
        }
        _context.Remove(fthread);
        _context.SaveChanges();
        return NoContent();
    }

}
