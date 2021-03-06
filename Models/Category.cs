namespace ShoppingifyAPI.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public Category(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
