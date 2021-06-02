using Microsoft.EntityFrameworkCore;
using ShoppingifyAPI.Models;

namespace ShoppingifyAPI.Context
{
    public class ApiContext : DbContext
    {
        public DbSet<Item> Items;
        public DbSet<Category> Categories;

        public ApiContext(DbContextOptions options) : base(options) { }
    }
}
