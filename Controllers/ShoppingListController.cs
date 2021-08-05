using ShoppingifyAPI.Context;
using ShoppingifyAPI.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ShoppingifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingListController : ControllerBase
    {
        private readonly ApiContext _context;

        public ShoppingListController(ApiContext context) => _context = context;

        [HttpGet]
        public List<ShoppingList> GetLists()
        {
            return _context.ShoppingLists.ToList();
        }

        [HttpGet("{listId}/items")]
        public List<Group<ShoppingListItem>> GetListItem([FromRoute] int listId)
        {
            var list = _context.ShoppingLists.Where(x => x.Id == listId).FirstOrDefault();

            var items = _context.ShoppingListItems
                .Where(x => x.ShoppingListId == list.Id)
                .Include(x => x.Item)
                .ThenInclude(x => x.Category)
                .ToList();

            return items.GroupBy(x => x.Item.Category.Name, (key, group) => new Group<ShoppingListItem>(key, group)).OrderBy(x => x.Key).ToList();
        }

        [HttpPost]
        public async Task<ActionResult<ShoppingList>> CreateList([FromBody] ShoppingList shoppingList)
        {
            await _context.ShoppingLists.AddAsync(shoppingList);
            await _context.SaveChangesAsync();

            return Created("", shoppingList);
        }

        [HttpDelete("{listId}")]
        public async Task<ActionResult> DeleteList([FromRoute] int listId)
        {
            var list = await _context.ShoppingLists.Where(x => x.Id == listId).FirstOrDefaultAsync();
            _context.ShoppingLists.Remove(list);

            return Ok();
        }

        [HttpPost("{listId}/add/{itemId}")]
        public ActionResult AddItemToList([FromRoute] int listId, [FromRoute] int itemId)
        {
            var item = _context.Items.Where(x => x.Id == itemId).FirstOrDefault();

            var listItem = new ShoppingListItem { Item = item, ItemId = itemId, ShoppingListId = listId, Quantity = 1 };

            _context.ShoppingListItems.Add(listItem);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPut("active/{listId}")]
        public ActionResult SetActiveList([FromRoute] int listId)
        {
            var activeList = _context.ShoppingLists.Where(x => x.Active).FirstOrDefault();

            activeList.Active = false;

            var selectedList = _context.ShoppingLists.Where(x => x.Id == listId).FirstOrDefault();
            selectedList.Active = true;

            _context.SaveChanges();

            return Ok();
        }
    }
}
