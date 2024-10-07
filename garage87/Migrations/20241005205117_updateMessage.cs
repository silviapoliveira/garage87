using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace garage87.Migrations
{
    public partial class updateMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RepliedBy",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "ReplyDate",
                table: "Message");

            migrationBuilder.RenameColumn(
                name: "ReplyMessage",
                table: "Message",
                newName: "Phone");

            migrationBuilder.AlterColumn<string>(
                name: "MessageDetail",
                table: "Message",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Message",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Message",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Message");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Message",
                newName: "ReplyMessage");

            migrationBuilder.AlterColumn<string>(
                name: "MessageDetail",
                table: "Message",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "RepliedBy",
                table: "Message",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReplyDate",
                table: "Message",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
