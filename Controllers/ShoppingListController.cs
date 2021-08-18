using ShoppingifyAPI.Context;
using ShoppingifyAPI.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ShoppingifyAPI.Models.Enums;
using System;

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

            if (list is null) return null;

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
            if (string.IsNullOrEmpty(shoppingList?.Name)) return BadRequest(new { error = "A list must have a name" });

            if (shoppingList.Date < DateTime.Today) return BadRequest(new { error = "List date must be at least today" });

            shoppingList.Active = false;
            shoppingList.Status = ListStatus.In_Progress;

            await _context.ShoppingLists.AddAsync(shoppingList);
            await _context.SaveChangesAsync();

            return Created("", shoppingList);
        }

        [HttpPost("{listId}/add/{itemId}")]
        public ActionResult AddItemToList([FromRoute] int listId, [FromRoute] int itemId)
        {
            var item = _context.Items.Where(x => x.Id == itemId).FirstOrDefault();

            if (item is null) return BadRequest(new { error = "Something went wrong" });

            var listItem = new ShoppingListItem { Item = item, ItemId = itemId, ShoppingListId = listId, Quantity = 1 };

            var itemAlreadyInList = _context.ShoppingListItems.Contains(listItem);

            if (itemAlreadyInList) return BadRequest(new { error = "Item is already on the list" });

            _context.ShoppingListItems.Add(listItem);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPut("{listId}/status")]
        public ActionResult ChangeListStatus([FromRoute] int listId, [FromQuery] ListStatus status)
        {
            var list = _context.ShoppingLists.Where(x => x.Id == listId).FirstOrDefault();

            list.Status = status;

            if (list.Active) list.Active = false;

            _context.SaveChanges();

            return Ok();
        }

        [HttpPut("active/{listId}")]
        public ActionResult SetActiveList([FromRoute] int listId)
        {
            var activeList = _context.ShoppingLists.Where(x => x.Active).FirstOrDefault();

            if (activeList is not null)
                activeList.Active = false;

            if (activeList?.Id == listId) return BadRequest(new { error = "List is already active" });

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
                return BadRequest(new { error = "Something went wrong" });

            if (quantity < 1)
                return BadRequest(new { error = "Item quantity cannot be lower than 1" });

            item.Quantity = quantity;

            _context.SaveChanges();

            return Ok();
        }

        [HttpDelete("{listId}")]
        public ActionResult DeleteList([FromRoute] int listId)
        {
            var list = _context.ShoppingLists.Where(x => x.Id == listId).FirstOrDefault();
            _context.ShoppingLists.Remove(list);
            _context.SaveChanges();

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
