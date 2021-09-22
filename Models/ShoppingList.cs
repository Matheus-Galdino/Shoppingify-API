using ShoppingifyAPI.Models.Enums;
using System;
using System.Collections.Generic;

namespace ShoppingifyAPI.Models
{
    public class ShoppingList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public DateTime Date { get; set; }
        public ListStatus Status { get; set; }
        public List<ShoppingListItem> Items { get; set; } = new List<ShoppingListItem>();

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
