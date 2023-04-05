using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.DAL.Migrations
{
    public partial class Addedindexesinprojecttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CompletionDate", "CreatedAt", "StartDate" },
                values: new object[] { new DateTime(2023, 7, 8, 17, 24, 18, 445, DateTimeKind.Utc).AddTicks(51), new DateTime(2023, 3, 8, 17, 24, 18, 445, DateTimeKind.Utc).AddTicks(49), new DateTime(2023, 3, 8, 17, 24, 18, 445, DateTimeKind.Utc).AddTicks(51) });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectStatus_ProjectPriority",
                table: "Projects",
                columns: new[] { "ProjectStatus", "ProjectPriority" });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_StartDate_CompletionDate",
                table: "Projects",
                columns: new[] { "StartDate", "CompletionDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectStatus_ProjectPriority",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_StartDate_CompletionDate",
                table: "Projects");

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CompletionDate", "CreatedAt", "StartDate" },
                values: new object[] { new DateTime(2023, 7, 8, 13, 32, 45, 111, DateTimeKind.Utc).AddTicks(8165), new DateTime(2023, 3, 8, 13, 32, 45, 111, DateTimeKind.Utc).AddTicks(8164), new DateTime(2023, 3, 8, 13, 32, 45, 111, DateTimeKind.Utc).AddTicks(8165) });
        }
    }
}
