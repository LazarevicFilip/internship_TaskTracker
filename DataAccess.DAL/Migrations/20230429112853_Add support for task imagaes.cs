using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.DAL.Migrations
{
    public partial class Addsupportfortaskimagaes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileUri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_Tasks_TaskId",
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
                values: new object[] { new DateTime(2023, 8, 29, 11, 28, 53, 31, DateTimeKind.Utc).AddTicks(8539), new DateTime(2023, 4, 29, 11, 28, 53, 31, DateTimeKind.Utc).AddTicks(8537), new DateTime(2023, 4, 29, 11, 28, 53, 31, DateTimeKind.Utc).AddTicks(8539) });

            migrationBuilder.CreateIndex(
                name: "IX_Files_TaskId",
                table: "Files",
                column: "TaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CompletionDate", "CreatedAt", "StartDate" },
                values: new object[] { new DateTime(2023, 8, 27, 10, 29, 33, 218, DateTimeKind.Utc).AddTicks(5699), new DateTime(2023, 4, 27, 10, 29, 33, 218, DateTimeKind.Utc).AddTicks(5697), new DateTime(2023, 4, 27, 10, 29, 33, 218, DateTimeKind.Utc).AddTicks(5698) });
        }
    }
}
