using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloverleafTrack.Migrations
{
    public partial class AddRelayEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FieldRelayEventId",
                table: "Performances",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RunningRelayEventId",
                table: "Performances",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FieldRelayEvents",
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
                    table.PrimaryKey("PK_FieldRelayEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RunningRelayEvents",
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
                    table.PrimaryKey("PK_RunningRelayEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Performances_FieldRelayEventId",
                table: "Performances",
                column: "FieldRelayEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Performances_RunningRelayEventId",
                table: "Performances",
                column: "RunningRelayEventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Performances_FieldRelayEvents_FieldRelayEventId",
                table: "Performances",
                column: "FieldRelayEventId",
                principalTable: "FieldRelayEvents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Performances_RunningRelayEvents_RunningRelayEventId",
                table: "Performances",
                column: "RunningRelayEventId",
                principalTable: "RunningRelayEvents",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Performances_FieldRelayEvents_FieldRelayEventId",
                table: "Performances");

            migrationBuilder.DropForeignKey(
                name: "FK_Performances_RunningRelayEvents_RunningRelayEventId",
                table: "Performances");

            migrationBuilder.DropTable(
                name: "FieldRelayEvents");

            migrationBuilder.DropTable(
                name: "RunningRelayEvents");

            migrationBuilder.DropIndex(
                name: "IX_Performances_FieldRelayEventId",
                table: "Performances");

            migrationBuilder.DropIndex(
                name: "IX_Performances_RunningRelayEventId",
                table: "Performances");

            migrationBuilder.DropColumn(
                name: "FieldRelayEventId",
                table: "Performances");

            migrationBuilder.DropColumn(
                name: "RunningRelayEventId",
                table: "Performances");
        }
    }
}
