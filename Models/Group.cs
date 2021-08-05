using System.Linq;
using System.Collections.Generic;

namespace ShoppingifyAPI.Models
{
    public class Group<T>
    {
        public string Key { get; set; }
        public List<T> Items { get; set; }

        public Group(string key, IEnumerable<T> items)
        {
            Key = key;
            Items = items.ToList();
        }
    }
}
