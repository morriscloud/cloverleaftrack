using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloverleafTrack.Migrations
{
    public partial class AddRunningEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RunningEventId",
                table: "Performances",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "FieldEvents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "RunningEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Environment = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunningEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Performances_RunningEventId",
                table: "Performances",
                column: "RunningEventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Performances_RunningEvents_RunningEventId",
                table: "Performances",
                column: "RunningEventId",
                principalTable: "RunningEvents",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Performances_RunningEvents_RunningEventId",
                table: "Performances");

            migrationBuilder.DropTable(
                name: "RunningEvents");

            migrationBuilder.DropIndex(
                name: "IX_Performances_RunningEventId",
                table: "Performances");

            migrationBuilder.DropColumn(
                name: "RunningEventId",
                table: "Performances");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "FieldEvents");
        }
    }
}
