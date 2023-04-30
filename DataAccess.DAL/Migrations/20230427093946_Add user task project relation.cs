using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.DAL.Migrations
{
    public partial class Addusertaskprojectrelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectUsers",
                table: "ProjectUsers");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProjectUsers",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ProjectUsers_UserId_ProjectId",
                table: "ProjectUsers",
                columns: new[] { "UserId", "ProjectId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectUsers",
                table: "ProjectUsers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ProjectUserTasks",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    ProjectUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectUserTasks", x => new { x.ProjectUserId, x.TaskId });
                    table.ForeignKey(
                        name: "FK_ProjectUserTasks_ProjectUsers_ProjectUserId",
                        column: x => x.ProjectUserId,
                        principalTable: "ProjectUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectUserTasks_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CompletionDate", "CreatedAt", "StartDate" },
                values: new object[] { new DateTime(2023, 8, 27, 9, 39, 46, 97, DateTimeKind.Utc).AddTicks(8119), new DateTime(2023, 4, 27, 9, 39, 46, 97, DateTimeKind.Utc).AddTicks(8115), new DateTime(2023, 4, 27, 9, 39, 46, 97, DateTimeKind.Utc).AddTicks(8118) });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUserTasks_TaskId",
                table: "ProjectUserTasks",
                column: "TaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectUserTasks");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ProjectUsers_UserId_ProjectId",
                table: "ProjectUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectUsers",
                table: "ProjectUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProjectUsers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectUsers",
                table: "ProjectUsers",
                columns: new[] { "UserId", "ProjectId" });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CompletionDate", "CreatedAt", "StartDate" },
                values: new object[] { new DateTime(2023, 8, 24, 16, 5, 30, 785, DateTimeKind.Utc).AddTicks(5905), new DateTime(2023, 4, 24, 16, 5, 30, 785, DateTimeKind.Utc).AddTicks(5904), new DateTime(2023, 4, 24, 16, 5, 30, 785, DateTimeKind.Utc).AddTicks(5905) });
        }
    }
}
