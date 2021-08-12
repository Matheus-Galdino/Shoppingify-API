using ShoppingifyAPI.Models;
using ShoppingifyAPI.Context;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ShoppingifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApiContext _context;

        public CategoryController(ApiContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<List<Category>>> GetAll() => await _context.Categories.ToListAsync();

        [HttpPost]
        public async Task<ActionResult<Category>> Create([FromBody] Category category)
        {
            if (string.IsNullOrEmpty(category?.Name)) return BadRequest(new { error = "Category must have a name" });

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return Created("", category);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromBody] Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
