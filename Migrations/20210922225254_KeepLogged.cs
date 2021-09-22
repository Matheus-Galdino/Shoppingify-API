using Microsoft.EntityFrameworkCore.Migrations;

namespace ShoppingifyAPI.Migrations
{
    public partial class KeepLogged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KeepUserLogged",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    UserHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeepUserLogged", x => new { x.UserId, x.UserHash });
                    table.ForeignKey(
                        name: "FK_KeepUserLogged_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KeepUserLogged");
        }
    }
}
