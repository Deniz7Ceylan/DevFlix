using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevFlix.Data;
using DevFlix.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace DevFlix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserFavoritesController : ControllerBase
    {
        private readonly DevFlixContext _context;
        private readonly SignInManager<DevFlixUser> _signInManager;

        public UserFavoritesController(SignInManager<DevFlixUser> signInManager, DevFlixContext context)
        {
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet("WithUserId")]
        public async Task<ActionResult<List<Media>>> GetUserFavourites(long userId)
        {
            var devFlixUser = await _signInManager.UserManager.FindByIdAsync(userId.ToString());

            if (_context.UserFavorites == null)
            {
                return NotFound();
            }

            var userFavorites = await _context.UserFavorites
             .Include(uf => uf.Media)
             .Where(uf => uf.UserId == devFlixUser.Id).Select(uf => uf.Media!.Name).ToListAsync();

            if (userFavorites.Count == 0)
            {
                return NotFound();
            }

            return Ok(userFavorites);
        }

        // GET: api/UserFavorites/5
        [HttpGet("{eMail}")]
        public async Task<ActionResult<List<UserFavorite>>> GetUserFavorites(string eMail)
        {
            var devFlixUser = await _signInManager.UserManager.FindByEmailAsync(eMail);

            if (devFlixUser == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            // Kullanıcının favori medyalarını getir
            var userFavorites = await _context.UserFavorites
             .Include(uf => uf.Media)
             .Where(uf => uf.UserId == devFlixUser.Id).Select(uf => uf.Media!.Name).ToListAsync();

            if (userFavorites == null || userFavorites.Count == 0)
            {
                return NotFound("Kullanıcının favorileri bulunamadı.");
            }

            return Ok(userFavorites);
        }

        // POST: api/UserFavorites
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserFavorite>> PostUserFavorite(UserFavorite userFavorite)
        {
            if (_context.UserFavorites == null)
            {
                return Problem("Entity set 'DevFlixContext.UserFavorites'  is null.");
            }
            _context.UserFavorites.Add(userFavorite);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserFavoriteExists(userFavorite.UserId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUserFavorite", new { id = userFavorite.UserId }, userFavorite);
        }

        // DELETE: api/UserFavorites/5
        [HttpDelete("{mediaId}")]
        [Authorize]
        public async Task<IActionResult> DeleteUserFavorite(int mediaId, long userId)
        {
            DevFlixUser? devFlixUser = _signInManager.UserManager.Users.Where(u => u.Id == userId).FirstOrDefault();

            if (_context.UserFavorites == null)
            {
                return NotFound();
            }

            var userFavorite = await _context.UserFavorites.Include(n => n.Media).FirstOrDefaultAsync(n => n.MediaId == mediaId && n.UserId == devFlixUser!.Id);

            if (userFavorite == null)
            {
                return NotFound("Favori bulunamadı.");
            }

            _context.UserFavorites.Remove(userFavorite);
            await _context.SaveChangesAsync();

            return Ok("Favori kaldırıldı.");
        }

        private bool UserFavoriteExists(long id)
        {
            return (_context.UserFavorites?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
