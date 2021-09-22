using Microsoft.EntityFrameworkCore;
using ShoppingifyAPI.Models;

namespace ShoppingifyAPI.Context
{
    public class ApiContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<KeepUserLogged> KeepUserLogged { get; set; }
        public DbSet<ShoppingListItem> ShoppingListItems { get; set; }

        public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShoppingListItem>().HasKey(shopI => new { shopI.ItemId, shopI.ShoppingListId });
            modelBuilder.Entity<ShoppingListItem>().Ignore(si => si.ShoppingList).ToTable("ShoppingListItem");

            modelBuilder.Entity<KeepUserLogged>().HasKey(x => new { x.UserId, x.UserHash });

            base.OnModelCreating(modelBuilder);
        }
    }
}
