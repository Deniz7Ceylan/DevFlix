using System.Security.Claims;
using DevFlix.Data;
using DevFlix.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevFlix.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DevFlixUsersController : ControllerBase
{
    public struct LogInModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    private readonly SignInManager<DevFlixUser> _signInManager;
    private readonly DevFlixContext _context;

    public DevFlixUsersController(SignInManager<DevFlixUser> signInManager, DevFlixContext context)
    {
        _signInManager = signInManager;
        _context = context;
    }

    // GET: api/DevFlixUsers
    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public ActionResult<List<DevFlixUser>> GetUsers(bool includePassive = true)
    {
        IQueryable<DevFlixUser> users = _signInManager.UserManager.Users;

        if (includePassive == false)
        {
            users = users.Where(u => u.Passive == false);
        }
        return users.AsNoTracking().ToList();
    }

    // GET: api/DevFlixUsers/5
    [HttpGet("{id}")]
    [Authorize]
    public ActionResult<DevFlixUser> GetDevFlixUser(long id)
    {
        DevFlixUser? devFlixUser = null;

        if (User.IsInRole("Administrator") == false)
        {
            if (User.FindFirstValue(ClaimTypes.NameIdentifier) != id.ToString())
            {
                return Unauthorized();
            }
        }
        devFlixUser = _signInManager.UserManager.Users.Where(u => u.Id == id).AsNoTracking().FirstOrDefault();

        if (devFlixUser == null)
        {
            return NotFound();
        }

        return devFlixUser;
    }

    // PUT: api/DevFlixUsers/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut]
    [Authorize]
    public ActionResult PutDevFlixUser(DevFlixUser devFlixUser)
    {
        DevFlixUser? user = null;

        if (User.IsInRole("ContentAdmin") == false)
        {
            if (User.FindFirstValue(ClaimTypes.NameIdentifier) != devFlixUser.Id.ToString())
            {
                return Unauthorized();
            }
        }
        user = _signInManager.UserManager.Users.Where(u => u.Id == devFlixUser.Id).FirstOrDefault();

        if (user == null)
        {
            return NotFound();
        }
        user.UserName = devFlixUser.UserName;
        user.PhoneNumber = devFlixUser.PhoneNumber;
        user.BirthDate = devFlixUser.BirthDate;
        user.Email = devFlixUser.Email;
        user.Name = devFlixUser.Name;
        _signInManager.UserManager.UpdateAsync(user).Wait();
        return Ok();
    }

    // POST: api/DevFlixUsers
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public ActionResult<string> PostDevFlixUser(DevFlixUser devFlixUser, string password)
    {
        //if (User.Identity!.IsAuthenticated == true)
        //{
        //    return BadRequest();
        //}
        IdentityResult identityResult = _signInManager.UserManager.CreateAsync(devFlixUser, password).Result;

        if (identityResult != IdentityResult.Success)
        {
            return identityResult.Errors.FirstOrDefault()!.Description;
        }
        return Ok();
    }

    // DELETE: api/DevFlixUsers/5
    [HttpDelete("{id}")]
    [Authorize]
    public ActionResult DeleteDevFlixUser(long id)
    {
        DevFlixUser? user = null;

        if (User.IsInRole("Administrator") == false)
        {
            if (User.FindFirstValue(ClaimTypes.NameIdentifier) != id.ToString())
            {
                return Unauthorized();
            }
        }
        user = _signInManager.UserManager.Users.Where(u => u.Id == id).FirstOrDefault();

        if (user == null)
        {
            return NotFound();
        }
        user.Passive = true;
        _signInManager.UserManager.UpdateAsync(user).Wait();
        return Ok();
    }

    [HttpPost("LogIn")]
    public ActionResult<List<Media>?> LogIn(LogInModel logInModel)
    {
        Microsoft.AspNetCore.Identity.SignInResult signInResult;
        DevFlixUser applicationUser = _signInManager.UserManager.FindByNameAsync(logInModel.UserName).Result;
        List<Media>? medias = null;
        List<UserFavorite> userFavorites;
        IGrouping<short, MediaCategory>? mediaCategories;
        IQueryable<Media> mediaQuery;
        IQueryable<int> userWatcheds;

        if (applicationUser == null)
        {
            return NotFound();
        }
        if (_signInManager.UserManager.IsInRoleAsync(applicationUser, "Administrator").Result == true || _signInManager.UserManager.IsInRoleAsync(applicationUser, "ContentAdmin").Result == true)
        {
            signInResult = _signInManager.PasswordSignInAsync(applicationUser, logInModel.Password, false, false).Result;
            if (signInResult.Succeeded == true)
            {
                return Ok("Admin giriş yaptı.");
            }
        }
        if (_context.UserPlans.Where(u => u.UserId == applicationUser.Id && u.EndDate >= DateTime.Today).Any() == false)
        {
            applicationUser.Passive = true;
            _signInManager.UserManager.UpdateAsync(applicationUser).Wait();
        }
        if (applicationUser.Passive == true)
        {
            return Content("Passive");
        }
        signInResult = _signInManager.PasswordSignInAsync(applicationUser, logInModel.Password, false, false).Result;
        if (signInResult.Succeeded == true)
        {
            //Kullanıcının favori olarak işaretlediği mediaları ve kategorilerini alıyoruz.
            userFavorites = _context.UserFavorites.Where(u => u.UserId == applicationUser.Id).Include(u => u.Media).Include(u => u.Media!.MediaCategories).ToList();

            //userFavorites içindeki media kategorilerini ayıklıyoruz (SelectMany)
            //Bunları kategori id'lerine göre grupluyoruz (GroupBy)
            //Her grupta kaç adet item olduğuna bakıp (m.Count())
            //Çoktan aza doğru sıralıyoruz (OrderByDescending)
            //En üstteki, yani en çok item'a sahip grubu seçiyoruz (FirstOrDefault)
            mediaCategories = userFavorites.SelectMany(u => u.Media!.MediaCategories!).GroupBy(m => m.CategoryId).OrderByDescending(m => m.Count()).FirstOrDefault();

            if (mediaCategories != null)
            {
                //Kullanıcının izlediği episode'lardan media'ya ulaşıp, sadece media id'lerini alıyoruz (Select)
                //Tekrar eden media id'leri eliyoruz (Distinct)
                userWatcheds = _context.UserWatcheds.Where(u => u.UserId == applicationUser.Id).Include(u => u.Episode).Select(u => u.Episode!.MediaId).Distinct();

                //Öneri yapmak için mediaCategories.Key'i yani kullanıcının en çok favorilediği kategori id'sini kullanıyoruz
                //Media listesini çekerken sadece bu kategoriye ait mediaların MediaCategories listesini dolduruyoruz (Include(m => m.MediaCategories!.Where(mc => mc.CategoryId == mediaCategories.Key)))
                //Diğer mediaların MediaCategories listeleri boş kalıyor
                //Sonrasında MediaCategories'i boş olmayan media'ları filtreliyoruz (m.MediaCategories!.Count > 0)
                //Ayrıca bu kategoriye giren fakat kullanıcının izlemiş olduklarını da dışarıda bırakıyoruz (userWatcheds.Contains(m.Id) == false)
                mediaQuery = _context.Medias.Include(m => m.MediaCategories!.Where(mc => mc.CategoryId == mediaCategories.Key)).Where(m => m.MediaCategories!.Count > 0 && userWatcheds.Contains(m.Id) == false);

                if (applicationUser.Restriction != null)
                {
                    //todo
                    //Son olarak, kullanıcı bir restrictiona sahipse seçilen media içerisinden bunları da çıkarmamız gerekiyor.
                    mediaQuery = mediaQuery.Include(m => m.MediaRestrictions!.Where(r => r.RestrictionId <= applicationUser.Restriction));
                }
                medias = mediaQuery.ToList();
            }
            //Populate medias
        }
        return medias;
    }

    [HttpPost("Logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok("Başarıyla çıkış yapıldı.");
    }
}