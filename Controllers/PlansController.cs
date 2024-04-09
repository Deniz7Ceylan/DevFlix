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
public class PlansController : ControllerBase
{
    private readonly DevFlixContext _context;

    public PlansController(DevFlixContext context)
    {
        _context = context;
    }

    // GET: api/Plans
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Plan>>> GetPlans()
    {
      if (_context.Plans == null)
      {
          return NotFound();
      }
        return await _context.Plans.ToListAsync();
    }

    // GET: api/Plans/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Plan>> GetPlan(short id)
    {
      if (_context.Plans == null)
      {
          return NotFound();
      }
        var plan = await _context.Plans.FindAsync(id);

        if (plan == null)
        {
            return NotFound();
        }

        return plan;
    }

    // PUT: api/Plans/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator, ContentAdmin")]
    public ActionResult PutPlan(short id, Plan plan)
    {
        if (id != plan.Id)
        {
            return BadRequest();
        }

        _context.Plans.Update(plan);
        _context.SaveChanges();
        return NoContent();
    }

    // POST: api/Plans
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "Administrator, ContentAdmin")]
    public ActionResult<Plan> PostPlan(Plan plan)
    {
        _context.Plans.Add(plan);
        _context.SaveChanges();
        return Ok();
    }

    // DELETE: api/Plans/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator, ContentAdmin")]
    public ActionResult DeletePlan(short id)
    {
        var plan = _context.Plans.Find(id);
        if (plan == null)
        {
            return NotFound();
        }
        _context.Plans.Remove(plan);
        _context.SaveChanges();
        return NoContent();
    }

    private bool PlanExists(short id)
    {
        return (_context.Plans?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
