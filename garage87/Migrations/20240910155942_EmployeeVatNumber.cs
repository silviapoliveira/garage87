using Microsoft.EntityFrameworkCore.Migrations;

namespace garage87.Migrations
{
    public partial class EmployeeVatNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VatNumber",
                table: "Employees",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_VatNumber",
                table: "Employees",
                column: "VatNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_VatNumber",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "VatNumber",
                table: "Employees");
        }
    }
}
