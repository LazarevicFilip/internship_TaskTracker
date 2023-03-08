using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.DAL.Migrations
{
    public partial class AddRowVersioncolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Tasks",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Projects",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CompletionDate", "CreatedAt", "StartDate" },
                values: new object[] { new DateTime(2023, 7, 8, 13, 32, 45, 111, DateTimeKind.Utc).AddTicks(8165), new DateTime(2023, 3, 8, 13, 32, 45, 111, DateTimeKind.Utc).AddTicks(8164), new DateTime(2023, 3, 8, 13, 32, 45, 111, DateTimeKind.Utc).AddTicks(8165) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Projects");

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CompletionDate", "CreatedAt", "StartDate" },
                values: new object[] { new DateTime(2023, 6, 22, 21, 37, 46, 932, DateTimeKind.Utc).AddTicks(1156), new DateTime(2023, 2, 22, 21, 37, 46, 932, DateTimeKind.Utc).AddTicks(1154), new DateTime(2023, 2, 22, 21, 37, 46, 932, DateTimeKind.Utc).AddTicks(1155) });
        }
    }
}
