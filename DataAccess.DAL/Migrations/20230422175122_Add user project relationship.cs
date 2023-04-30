using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.DAL.Migrations
{
    public partial class Adduserprojectrelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectUsers",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectUsers", x => new { x.UserId, x.ProjectId });
                    table.ForeignKey(
                        name: "FK_ProjectUsers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CompletionDate", "CreatedAt", "StartDate" },
                values: new object[] { new DateTime(2023, 8, 22, 17, 51, 22, 58, DateTimeKind.Utc).AddTicks(2798), new DateTime(2023, 4, 22, 17, 51, 22, 58, DateTimeKind.Utc).AddTicks(2797), new DateTime(2023, 4, 22, 17, 51, 22, 58, DateTimeKind.Utc).AddTicks(2798) });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_ProjectId",
                table: "ProjectUsers",
                column: "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectUsers");

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CompletionDate", "CreatedAt", "StartDate" },
                values: new object[] { new DateTime(2023, 7, 8, 17, 24, 18, 445, DateTimeKind.Utc).AddTicks(51), new DateTime(2023, 3, 8, 17, 24, 18, 445, DateTimeKind.Utc).AddTicks(49), new DateTime(2023, 3, 8, 17, 24, 18, 445, DateTimeKind.Utc).AddTicks(51) });
        }
    }
}
