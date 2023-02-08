using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.DAL.Migrations
{
    public partial class addseeddata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "CompletionDate", "CreatedAt", "DeletedAt", "Name", "ProjectPriority", "ProjectStatus", "StartDate", "UpdatedAt" },
                values: new object[] { 1, new DateTime(2023, 6, 8, 11, 30, 57, 568, DateTimeKind.Utc).AddTicks(5606), new DateTime(2023, 2, 8, 11, 30, 57, 568, DateTimeKind.Utc).AddTicks(5605), null, "Project from seeder.", 3, 0, new DateTime(2023, 2, 8, 11, 30, 57, 568, DateTimeKind.Utc).AddTicks(5605), null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
