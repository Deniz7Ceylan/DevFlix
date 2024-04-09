using DevFlix.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevFlix.Data;
using Microsoft.AspNetCore.Authorization;

namespace DevFlix.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DirectorsController : ControllerBase
{
    private readonly DevFlixContext _context;

    public DirectorsController(DevFlixContext context)
    {
        _context = context;
    }

    // GET: api/Directors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Director>>> GetDirectors()
    {
        if (_context.Directors == null)
        {
            return NotFound();
        }
        return await _context.Directors.ToListAsync();
    }

    // GET: api/Directors/5
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Director>> GetDirector(int id)
    {
        if (_context.Directors == null)
        {
            return NotFound();
        }
        var director = _context.Directors.Where(d => d.Id == id).Include(d => d.MediaDirectors).AsNoTracking().FirstOrDefault();

        if (director == null)
        {
            return NotFound();
        }

        return director;
    }

    // PUT: api/Directors/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator, ContentAdmin")]
    public async Task<IActionResult> PutDirector(int id, Director director)
    {
        if (id != director.Id)
        {
            return BadRequest();
        }

        _context.Directors.Update(director);
        _context.SaveChanges();

        return Ok("Yönetmen güncellendi.");
    }

    // POST: api/Directors
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "Administrator, ContentAdmin")]
    public async Task<ActionResult<Director>> PostDirector(Director director)
    {
        if (_context.Directors == null)
        {
            return Problem("Entity set 'DevFlixContext.Directors'  is null.");
        }
        _context.Directors.Add(director);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetDirector", new { id = director.Id }, director);
    }

    // DELETE: api/Directors/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator, ContentAdmin")]
    public async Task<IActionResult> DeleteDirector(int id)
    {
        if (_context.Directors == null)
        {
            return NotFound();
        }
        Director? director = await _context.Directors.FindAsync(id);
        if (director == null)
        {
            return NotFound("Yönetmen bulunamadı.");
        }

        var mediaDirectors = _context.MediaDirectors.Where(m => m.DirectorId == id);
        foreach (var mediaDirector in mediaDirectors)
        {
            var media = _context.Medias.FindAsync(mediaDirector.MediaId).Result;
            if (media != null)
            {
                media.Passive = true;
                _context.Medias.Update(media);
            }
        }
        _context.SaveChanges();
        return Ok();
    }

    private bool DirectorExists(int id)
    {
        return (_context.Directors?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}