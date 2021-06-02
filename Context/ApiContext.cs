using Microsoft.EntityFrameworkCore;
using ShoppingifyAPI.Models;

namespace ShoppingifyAPI.Context
{
    public class ApiContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }

        public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }
    }
}
