﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevFlix.Data;
using DevFlix.Models;

namespace DevFlix.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly DevFlixContext _context;

    public CategoriesController(DevFlixContext context)
    {
        _context = context;
    }

    // GET: api/Categories
    [HttpGet]
    public ActionResult<List<Category>> GetCategories()
    {
        return _context.Categories.AsNoTracking().ToList();
    }

    // GET: api/Categories/5
    [HttpGet("{id}")]
    [Authorize]
    public ActionResult<Category> GetCategory(short id)
    {
        Category? category = _context.Categories.Find(id);

        if (category == null)
        {
            return NotFound();
        }

        return category;
    }

    // PUT: api/Categories/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut]
    [Authorize(Roles = "ContentAdmin")]
    public void PutCategory(Category category)
    {
        _context.Categories.Update(category);

        _context.SaveChanges();
    }

    // POST: api/Categories
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "ContentAdmin")]
    public short PostCategory(Category category)
    {
        _context.Categories.Add(category);
        _context.SaveChanges();

        return category.Id;
    }

    // DELETE: api/Categories/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "ContentAdmin")]
    public IActionResult DeleteCategory(short id)
    {
        var category = _context.Categories.Find(id);
        if (category == null)
        {
            return NotFound();
        }

        _context.Categories.Remove(category);
        _context.SaveChanges();

        return NoContent();
    }
}
