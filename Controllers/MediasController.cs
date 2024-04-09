using DevFlix.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevFlix.Data;
using Microsoft.AspNetCore.Authorization;

namespace DevFlix.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MediasController : ControllerBase
{
    private readonly DevFlixContext _context;

    public MediasController(DevFlixContext context)
    {
        _context = context;
    }

    // GET: api/Medias
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Media>>> GetMedias()
    {
        if (_context.Medias == null)
        {
            return NotFound();
        }
        return await _context.Medias.ToListAsync();
    }

    // GET: api/Medias/5
    [HttpGet("{id}")]
    [Authorize]
    public ActionResult<Media> GetMedia(int id)
    {
        Media? media = _context.Medias.Find(id);

        List<MediaCategory>? mediaCategory = _context.MediaCategories.Where(u => u.MediaId == id).Include(u => u.Category).ToList();
        List<MediaDirector>? mediaDirector = _context.MediaDirectors.Where(d => d.MediaId == id).Include(d => d.Director).ToList();
        List<MediaStar>? mediaStar = _context.MediaStars.Where(d => d.MediaId == id).Include(d => d.Star).ToList();

        if (media == null)
        {
            return NotFound();
        }
        return media;
    }

    [HttpGet("category/{categoryId}")]
    [Authorize]
    public ActionResult<List<MediaCategory>> GetMediaCategory(short categoryId)
    {
        var media = _context.MediaCategories.Where(mc => mc.CategoryId == categoryId).Include(mc => mc.Media).ToList();

        return media;
    }

    // PUT: api/Medias/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator, ContentAdmin")]
    public ActionResult PutMedia(int id, Media media)
    {
        Media? mediaToUpdate = null;
        if (mediaToUpdate == null)
        {
            return NotFound();
        }
        if (id != media.Id)
        {
            return BadRequest();
        }

        mediaToUpdate.Name = media.Name;
        mediaToUpdate.Description = media.Description;

        _context.Medias.Update(media);
        _context.SaveChanges();

        return Ok("Medya güncellendi.");
    }

    // POST: api/Medias
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "Administrator, ContentAdmin")]
    public async Task<ActionResult<Media>> PostMedia(Media media)
    {
        if (_context.Medias == null)
        {
            return Problem("Entity set 'DevFlixContext.Medias'  is null.");
        }
        _context.Medias.Add(media);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetMedia", new { id = media.Id }, media);
    }

    // DELETE: api/Medias/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator, ContentAdmin")]
    public ActionResult DeleteMedia(int id)
    {
        var media = _context.Medias.Find(id);
        if (media == null)
        {
            return NotFound();
        }
        media.Passive = true;
        _context.Medias.Update(media);
        _context.SaveChanges();

        return Ok("Medya silindi.");
    }

    private bool MediaExists(int id)
    {
        return (_context.Medias?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}