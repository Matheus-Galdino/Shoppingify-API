using System.Linq;
using System.Collections.Generic;

namespace ShoppingifyAPI.Models
{
    public class ItemGroup
    {
        public string Key { get; set; }
        public List<Item> Items { get; set; }

        public ItemGroup(string key, IEnumerable<Item> items)
        {
            this.Key = key;
            this.Items = items.ToList();
        }
    }
}
