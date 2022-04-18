using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloverleafTrack.Migrations
{
    public partial class AddFieldEventsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FieldEventId",
                table: "Performances",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FieldEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Performances_FieldEventId",
                table: "Performances",
                column: "FieldEventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Performances_FieldEvents_FieldEventId",
                table: "Performances",
                column: "FieldEventId",
                principalTable: "FieldEvents",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Performances_FieldEvents_FieldEventId",
                table: "Performances");

            migrationBuilder.DropTable(
                name: "FieldEvents");

            migrationBuilder.DropIndex(
                name: "IX_Performances_FieldEventId",
                table: "Performances");

            migrationBuilder.DropColumn(
                name: "FieldEventId",
                table: "Performances");
        }
    }
}
