namespace ShoppingifyAPI.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public string ImageUrl { get; set; }
        public Category Category { get; set; }       

        public Item(string name, string note, string imageUrl, Category category)
        {
            Name = name;
            Note = note;
            ImageUrl = imageUrl;
            Category = category;
        }

        public Item(int id, string name, string note, string imageUrl, Category category)
        {
            Id = id;
            Name = name;
            Note = note;
            ImageUrl = imageUrl;
            Category = category;
        }
    }
}
