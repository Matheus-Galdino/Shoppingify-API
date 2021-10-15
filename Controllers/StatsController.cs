using System;
using System.Linq;
using ShoppingifyAPI.Models;
using ShoppingifyAPI.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace ShoppingifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly ApiContext _context;

        private int AuthedUserId => int.Parse(User.FindFirst("Id").Value);

        public StatsController(ApiContext context) => _context = context;

        [HttpGet("top/items")]
        [Authorize]
        public ActionResult<List<Stat>> GetTopItems()
        {
            var totalItems = _context.ShoppingLists.Where(x => x.UserId == AuthedUserId).Include(x => x.Items).Sum(x => x.Items.Sum(item => item.Quantity));                     

            var topItems = _context.ShoppingListItems
                .Include(x => x.Item)
                .Where(x => x.Item.UserId == AuthedUserId)
                .GroupBy(x => x.Item.Name).Select(x => new Stat
                {
                    Key = x.Key,
                    Amount = x.Sum(s => s.Quantity),
                    Percentage = Math.Round(x.Sum(s => s.Quantity) * 100m / totalItems)
                }).OrderByDescending(x => x.Percentage).Take(3).ToList();                                        

            return topItems;
        }

        [HttpGet("top/categories")]
        [Authorize]
        public ActionResult<List<Stat>> GetTopCategories()
        {
            var totalItems = _context.ShoppingLists.Where(x => x.UserId == AuthedUserId).Include(x => x.Items).Sum(x => x.Items.Sum(item => item.Quantity));

            var topItems = _context.ShoppingListItems
                .Where(x => x.Item.UserId == AuthedUserId)
                .Include(x => x.Item).Include(x => x.Item.Category)
                .GroupBy(x => x.Item.Category.Name).Select(x => new Stat
                {
                    Key = x.Key,
                    Amount = x.Sum(s => s.Quantity),
                    Percentage = Math.Round(x.Sum(s => s.Quantity) * 100m / totalItems)
                }).OrderByDescending(x => x.Percentage).Take(3).ToList();

            return topItems;
        }

        [HttpGet("monthly")]
        [Authorize]
        public ActionResult<List<Stat>> MonthlySummary()
        {
            var months = new string[] { "", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

            var monthly = _context.ShoppingLists.Include(x => x.Items)
                .OrderBy(x => x.Date)
                .Where(x => x.UserId == AuthedUserId).ToList()
                .GroupBy(x => x.Date.Month, (key, group) => new Stat
                {
                    Key = months[key],
                    Amount = group.Sum(s => s.Items.Sum(i => i.Quantity))
                }).ToList();

            return monthly;
        }
    }
}
