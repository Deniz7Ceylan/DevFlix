using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevFlix.Data;
using DevFlix.Models;
using Microsoft.AspNetCore.Authorization;

namespace DevFlix.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StarsController : ControllerBase
{
    private readonly DevFlixContext _context;

    public StarsController(DevFlixContext context)
    {
        _context = context;
    }

    // GET: api/Stars
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Star>>> GetStars()
    {
      if (_context.Stars == null)
      {
          return NotFound();
      }
        return await _context.Stars.ToListAsync();
    }

    // GET: api/Stars/5
    [HttpGet("{id}")]
    [Authorize]
    public ActionResult<Star> GetStar(int id)
    {
        var star = _context.Stars.Where(s => s.Id == id).Include(s => s.MediaStars).AsNoTracking().FirstOrDefault();
        if (star == null)
        {
            return NotFound();
        }
        return star;
    }

    // PUT: api/Stars/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator, ContentAdmin")]
    public ActionResult PutStar(int id, Star star)
    {
        if (id != star.Id)
        {
            return BadRequest();
        }

        _context.Stars.Update(star);
        _context.SaveChanges();

        return Ok();
    }

    // POST: api/Stars
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "Administrator, ContentAdmin")]
    public ActionResult<Star> PostStar(Star star)
    {
        _context.Stars.Add(star);
        _context.SaveChanges();

        return Ok("Oyuncu eklendi.");
    }

    // DELETE: api/Stars/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator, ContentAdmin")]
    public ActionResult DeleteStar(int id)
    {
        Star? star = _context.Stars.FindAsync(id).Result;
        if (star == null)
        {
            return NotFound("Star not found.");
        }

        var mediaStars = _context.MediaStars.Where(ms => ms.StarId == id);
        foreach (var mediaStar in mediaStars)
        {
            var media = _context.Medias.FindAsync(mediaStar.MediaId).Result;
            if (media != null)
            {
                media.Passive = true;
                _context.Medias.Update(media);
            }
        }
        _context.SaveChanges();
        return Ok();
    }

    private bool StarExists(int id)
    {
        return (_context.Stars?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
