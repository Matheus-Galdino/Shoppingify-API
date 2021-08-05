using Microsoft.EntityFrameworkCore.Migrations;

namespace ShoppingifyAPI.Migrations
{
    public partial class ListStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ShoppingLists",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ShoppingLists");
        }
    }
}
