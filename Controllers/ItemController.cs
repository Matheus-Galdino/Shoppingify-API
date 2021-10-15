using ShoppingifyAPI.Models;
using ShoppingifyAPI.Context;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;

namespace ShoppingifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ApiContext _context;

        private int AuthedUserId => int.Parse(User.FindFirst("Id").Value);

        public ItemController(ApiContext context) => _context = context;

        [HttpGet]
        [Authorize]
        public ActionResult<List<Group<Item>>> GetAll()
        {
            var items = _context.Items.Include(x => x.Category).ToList();

            var groupedItems = items
                .Where(x => x.UserId == AuthedUserId)
                .GroupBy(x => x.Category, (key, group) => new Group<Item>(key.Name, group))
                .OrderBy(x => x.Key)
                .ToList();

            return groupedItems;
        }

        [HttpPost]
        [Authorize]
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

            item.UserId = AuthedUserId;

            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();

            return Created("", item);
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> Delete([FromBody] Item item)
        {
            item.UserId = AuthedUserId;

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
