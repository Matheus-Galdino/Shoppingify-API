using ShoppingifyAPI.Models;
using ShoppingifyAPI.Context;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace ShoppingifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApiContext _context;

        private int AuthedUserId => int.Parse(User.FindFirst("Id").Value);

        public CategoryController(ApiContext context) => _context = context;

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Category>>> GetAll() => await _context.Categories.Where(x => x.UserId == AuthedUserId).ToListAsync();

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Category>> Create([FromBody] Category category)
        {
            if (string.IsNullOrEmpty(category?.Name)) return BadRequest(new { error = "Category must have a name" });

            category.UserId = AuthedUserId;

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return Created("", category);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete([FromBody] Category category)
        {
            category.UserId = AuthedUserId;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
