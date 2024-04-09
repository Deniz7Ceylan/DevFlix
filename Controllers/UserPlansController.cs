using DevFlix.Data;
using DevFlix.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevFlix.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserPlansController : ControllerBase
{
    private readonly DevFlixContext _context;
    private readonly UserManager<DevFlixUser> _userManager;

    public UserPlansController(DevFlixContext context, UserManager<DevFlixUser> _userManager)
    {
        _context = context;
    }

    // GET: api/UserPlans
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<UserPlan>>> GetUserPlans()
    {
        if (_context.UserPlans == null)
        {
            return NotFound();
        }
        return await _context.UserPlans.ToListAsync();
    }

    // GET: api/UserPlans/5
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<UserPlan>> GetUserPlan(long id)
    {
        if (_context.UserPlans == null)
        {
            return NotFound();
        }
        var userPlan = await _context.UserPlans.FindAsync(id);

        if (userPlan == null)
        {
            return NotFound();
        }

        return userPlan;
    }

    // PUT: api/UserPlans/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUserPlan(long id, UserPlan userPlan)
    {
        if (id != userPlan.Id)
        {
            return BadRequest();
        }

        _context.Entry(userPlan).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserPlanExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/UserPlans
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize]
    public void PostUserPlan(long userId, short planId)
    {
        Plan plan = _context.Plans.Find(planId)!;

        if (plan != null)
        {
            UserPlan userPlan = new UserPlan
            {
                UserId = userId,
                PlanId = planId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(1)
            };

            _context.UserPlans.Add(userPlan);
            _context.SaveChanges();
        }
    }

    // DELETE: api/UserPlans/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserPlan(long id)
    {
        if (_context.UserPlans == null)
        {
            return NotFound();
        }
        var userPlan = await _context.UserPlans.FindAsync(id);
        if (userPlan == null)
        {
            return NotFound();
        }

        _context.UserPlans.Remove(userPlan);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UserPlanExists(long id)
    {
        return (_context.UserPlans?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}