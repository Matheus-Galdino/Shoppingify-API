using ShoppingifyAPI.Context;
using ShoppingifyAPI.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ShoppingifyAPI.Models.Enums;

namespace ShoppingifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingListController : ControllerBase
    {
        private readonly ApiContext _context;

        public ShoppingListController(ApiContext context) => _context = context;

        [HttpGet]
        public List<ShoppingList> GetLists() => _context.ShoppingLists.OrderBy(x => x.Status).ToList();

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

        [HttpPost("{listId}/add/{itemId}")]
        public ActionResult AddItemToList([FromRoute] int listId, [FromRoute] int itemId)
        {
            var item = _context.Items.Where(x => x.Id == itemId).FirstOrDefault();

            var listItem = new ShoppingListItem { Item = item, ItemId = itemId, ShoppingListId = listId, Quantity = 1 };

            _context.ShoppingListItems.Add(listItem);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPut("{listId}/status")]
        public ActionResult ChangeListStatus([FromRoute] int listId, [FromQuery] ListStatus status)
        {
            var list = _context.ShoppingLists.Where(x => x.Id == listId).FirstOrDefault();

            list.Status = status;

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

        [HttpPut("{listId}/item/{itemId}")]
        public ActionResult UpdateItemQuantity([FromRoute] int listId, [FromRoute] int itemId, [FromQuery] int quantity)
        {
            var item = _context.ShoppingListItems.Where(x => x.ItemId == itemId && x.ShoppingListId == listId).FirstOrDefault();

            if (item is null)
                return BadRequest(new { error = "Not list item found with given IDs" });

            item.Quantity = quantity;

            _context.SaveChanges();

            return Ok();
        }

        [HttpDelete("{listId}")]
        public ActionResult DeleteList([FromRoute] int listId)
        {
            var list = _context.ShoppingLists.Where(x => x.Id == listId).FirstOrDefault();
            _context.ShoppingLists.Remove(list);

            return Ok();
        }

        [HttpDelete("{listId}/item/{itemId}")]
        public ActionResult RemoveItemFromList([FromRoute] int listId, [FromRoute] int itemId)
        {
            var item = _context.ShoppingListItems.Where(x => x.ItemId == itemId && x.ShoppingListId == listId).FirstOrDefault();

            _context.ShoppingListItems.Remove(item);
            _context.SaveChanges();

            return Ok();
        }
    }
}
