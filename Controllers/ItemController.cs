using ShoppingifyAPI.Models;
using ShoppingifyAPI.Context;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            //max chars: 300
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
