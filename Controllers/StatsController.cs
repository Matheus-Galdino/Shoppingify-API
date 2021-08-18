using System;
using System.Linq;
using ShoppingifyAPI.Models;
using ShoppingifyAPI.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ShoppingifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly ApiContext _context;

        public StatsController(ApiContext context) => _context = context;

        [HttpGet("top/items")]
        public ActionResult<List<Stat>> GetTopItems()
        {
            var totalItems = _context.ShoppingListItems.Sum(x => x.Quantity);

            var topItems = _context.ShoppingListItems.Include(x => x.Item).GroupBy(x => x.Item.Name).Select(x => new Stat
            {
                Key = x.Key,
                Amount = x.Sum(s => s.Quantity),
                Percentage = Math.Round(x.Sum(s => s.Quantity) * 100m / totalItems)
            }).Take(3).OrderByDescending(x => x.Percentage).ToList();

            return topItems;
        }

        [HttpGet("top/categories")]
        public ActionResult<List<Stat>> GetTopCategories()
        {
            var totalItems = _context.ShoppingListItems.Sum(x => x.Quantity);

            var topItems = _context.ShoppingListItems.Include(x => x.Item).Include(x => x.Item.Category).GroupBy(x => x.Item.Category.Name).Select(x => new Stat
            {
                Key = x.Key,
                Amount = x.Sum(s => s.Quantity),
                Percentage = Math.Round(x.Sum(s => s.Quantity) * 100m / totalItems)
            }).Take(3).OrderByDescending(x => x.Percentage).ToList();

            return topItems;
        }

        [HttpGet("monthly")]
        public ActionResult<List<Stat>> MonthlySummary()
        {
            var months = new string[] {"", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

            var monthly = _context.ShoppingLists.Include(x => x.Items)
                .OrderBy(x => x.Date).ToList()
                .GroupBy(x => x.Date.Month, (key, group) => new Stat
                {
                    Key = months[key],
                    Amount = group.Sum(s => s.Items.Sum(i => i.Quantity))
                }).ToList();

            return monthly;
        }
    }
}
