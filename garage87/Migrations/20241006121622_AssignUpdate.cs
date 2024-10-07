using Microsoft.EntityFrameworkCore.Migrations;

namespace garage87.Migrations
{
    public partial class AssignUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignId",
                table: "Notifications",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignId",
                table: "Notifications");
        }
    }
}
