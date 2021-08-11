using ShoppingifyAPI.Models;
using ShoppingifyAPI.Context;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.RegularExpressions;

namespace ShoppingifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ApiContext _context;

        public ItemController(ApiContext context) => _context = context;

        [HttpGet]
        public ActionResult<List<Group<Item>>> GetAll()
        {
            var items = _context.Items.Include(x => x.Category).ToList();

            var groupedItems = items
                .GroupBy(x => x.Category, (key, group) => new Group<Item>(key.Name, group))
                .OrderBy(x => x.Key)
                .ToList();

            return groupedItems;
        }

        [HttpPost]
        public async Task<ActionResult<Item>> Create([FromBody] Item item)
        {
            if (string.IsNullOrEmpty(item.Name?.Trim())) return BadRequest(new { error = "Item name must not be empty" });

            if (item.Note?.Length > 300) return BadRequest(new { error = "Item note cannot have more than 300 chars" });

            if (!string.IsNullOrEmpty(item.ImageUrl?.Trim()))
            {
                var regex = new Regex(@"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()!@:%_\+.~#?&\/\/=]*)");
                var match = regex.Match(item.ImageUrl);

                if (!match.Success)
                    return BadRequest(new { error = "Item image url is not a valid url" });
            }

            var category = _context.Categories.Where(x => x.Id == item.CategoryId).FirstOrDefault();

            if (category is null) return BadRequest(new { error = "Invalid category" });

            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();

            return Created("", item);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete([FromBody] Item item)
        {
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
