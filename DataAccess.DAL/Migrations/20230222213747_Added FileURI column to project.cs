using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.DAL.Migrations
{
    public partial class AddedFileURIcolumntoproject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileURI",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "CompletionDate", "CreatedAt", "DeletedAt", "Name", "ProjectPriority", "ProjectStatus", "StartDate", "UpdatedAt" },
                values: new object[] { 1, new DateTime(2023, 6, 22, 21, 37, 46, 932, DateTimeKind.Utc).AddTicks(1156), new DateTime(2023, 2, 22, 21, 37, 46, 932, DateTimeKind.Utc).AddTicks(1154), null, "Project from seeder.", 3, 0, new DateTime(2023, 2, 22, 21, 37, 46, 932, DateTimeKind.Utc).AddTicks(1155), null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "FileURI",
                table: "Projects");
        }
    }
}
