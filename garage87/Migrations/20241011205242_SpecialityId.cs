using Microsoft.EntityFrameworkCore.Migrations;

namespace garage87.Migrations
{
    public partial class SpecialityId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Specialities_SpecialitiesId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_SpecialitiesId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SpecialitiesId",
                table: "Employees");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_SpecialityId",
                table: "Employees",
                column: "SpecialityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Specialities_SpecialityId",
                table: "Employees",
                column: "SpecialityId",
                principalTable: "Specialities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Specialities_SpecialityId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_SpecialityId",
                table: "Employees");

            migrationBuilder.AddColumn<int>(
                name: "SpecialitiesId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_SpecialitiesId",
                table: "Employees",
                column: "SpecialitiesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Specialities_SpecialitiesId",
                table: "Employees",
                column: "SpecialitiesId",
                principalTable: "Specialities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
