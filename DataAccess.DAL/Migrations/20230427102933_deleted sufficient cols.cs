using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.DAL.Migrations
{
    public partial class deletedsufficientcols : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Tasks");

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CompletionDate", "CreatedAt", "StartDate" },
                values: new object[] { new DateTime(2023, 8, 27, 10, 29, 33, 218, DateTimeKind.Utc).AddTicks(5699), new DateTime(2023, 4, 27, 10, 29, 33, 218, DateTimeKind.Utc).AddTicks(5697), new DateTime(2023, 4, 27, 10, 29, 33, 218, DateTimeKind.Utc).AddTicks(5698) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CompletionDate", "CreatedAt", "StartDate" },
                values: new object[] { new DateTime(2023, 8, 27, 9, 39, 46, 97, DateTimeKind.Utc).AddTicks(8119), new DateTime(2023, 4, 27, 9, 39, 46, 97, DateTimeKind.Utc).AddTicks(8115), new DateTime(2023, 4, 27, 9, 39, 46, 97, DateTimeKind.Utc).AddTicks(8118) });
        }
    }
}
