namespace ShoppingifyAPI.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public string ImageUrl { get; set; }
        public Category Category { get; set; }
    }
}
