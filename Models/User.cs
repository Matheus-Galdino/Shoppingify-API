using System.Collections.Generic;

namespace ShoppingifyAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Category> Categories { get; set; }
        public List<Item> Items { get; set; }
        public List<ShoppingList> Lists { get; set; }
    }
}
