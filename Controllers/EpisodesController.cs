using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevFlix.Data;
using DevFlix.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DevFlix.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EpisodesController : ControllerBase
{
    private readonly DevFlixContext _context;

    public EpisodesController(DevFlixContext context)
    {
        _context = context;
    }

    // GET: api/Episodes
    [HttpGet]
    [Authorize]
    public ActionResult<List<Episode>> GetEpisodes(int mediaId, byte seasonNumber)
    {
        return _context.Episodes.Where(e => e.MediaId == mediaId && e.SeasonNumber == seasonNumber).OrderBy(e => e.EpisodeNumber).AsNoTracking().ToList();
    }

    // GET: api/Episodes/5
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Episode>> GetEpisode(int mediaId, byte seasonNumber, short episodeNumber)
    {
        var result = _context.Episodes.Where(e => e.MediaId == mediaId && e.SeasonNumber == seasonNumber && e.EpisodeNumber == episodeNumber).FirstOrDefault();

        if (result != null)
        {
            return result;
        }
        return NotFound("İçerik bulunamadı.");
    }

    [HttpGet("Watch")]
    [Authorize]
    public void Watch(long id)
    {
        //Find logged in user.
        //Check age
        //If age is less than 18
        //Get media restrictions via episode
        //Check if the user is permitted to view the episode
        UserWatched userWatched = new UserWatched();
        Episode episode = _context.Episodes.Find(id)!;

        try
        {
            userWatched.UserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            userWatched.EpisodeId = id;
            _context.UserWatcheds.Add(userWatched);
            episode.ViewCount++;
            _context.Episodes.Update(episode);
            _context.SaveChanges();
            //İlk izlenmede artar
        }
        catch (Exception ex)
        {
            throw;
        }

        //her izlenmede artar
        episode.ViewCount++;
        _context.Episodes.Update(episode);
        _context.SaveChanges();
    }

    // PUT: api/Episodes/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator, ContentAdmin")]
    public async Task<IActionResult> PutEpisode(long id, Episode episode)
    {
        if (id != episode.Id)
        {
            return BadRequest();
        }

        var episodeToUpdate = await _context.Episodes.FirstOrDefaultAsync(e => e.Id == id);
        if (episodeToUpdate == null)
        {
            return NotFound("Güncellenecek içerik bulunamadı.");
        }
        episodeToUpdate.Title = episode.Title;
        episodeToUpdate.Description = episode.Description;
        episodeToUpdate.SeasonNumber = episode.SeasonNumber;
        episodeToUpdate.EpisodeNumber = episode.EpisodeNumber;
        episodeToUpdate.Duration = episode.Duration;
        episodeToUpdate.Passive = episode.Passive;

        _context.Episodes.Update(episodeToUpdate);
        await _context.SaveChangesAsync();

        return Ok();
    }

    // POST: api/Episodes
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "Administrator, ContentAdmin")]
    public async Task<ActionResult<Episode>> PostEpisode(Episode episode)
    {
        episode.ViewCount = 0;
        var media = await _context.Medias.FindAsync(episode.MediaId);

        if (media == null)
        {
            return BadRequest("Belirtilen MediaId'ye sahip bir medya bulunamadı.");
        }
        episode.Media = media;
        _context.Episodes.Add(episode);
        await _context.SaveChangesAsync();

        return Ok(episode);
    }

    // DELETE: api/Episodes/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator, ContentAdmin")]
    public async Task<IActionResult> DeleteEpisode(long id)
    {
        var episode = _context.Episodes.FindAsync(id).Result;
        if (episode == null)
        {
            return NotFound();
        }

        episode.Passive = true;
        _context.Episodes.Update(episode);
        _context.SaveChangesAsync();

        return Ok("İçerik silindi.");
    }

    private bool EpisodeExists(long id)
    {
        return (_context.Episodes?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}