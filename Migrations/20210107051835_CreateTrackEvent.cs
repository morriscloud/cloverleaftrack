using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CloverleafTrack.Migrations
{
    public partial class CreateTrackEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventName",
                table: "Performances");

            migrationBuilder.DropColumn(
                name: "Place",
                table: "Performances");

            migrationBuilder.DropColumn(
                name: "RunningEvent",
                table: "Performances");

            migrationBuilder.AddColumn<Guid>(
                name: "TrackEventId",
                table: "Performances",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "TrackEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<bool>(type: "bit", nullable: false),
                    RunningEvent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Performances_TrackEventId",
                table: "Performances",
                column: "TrackEventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Performances_TrackEvents_TrackEventId",
                table: "Performances",
                column: "TrackEventId",
                principalTable: "TrackEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Performances_TrackEvents_TrackEventId",
                table: "Performances");

            migrationBuilder.DropTable(
                name: "TrackEvents");

            migrationBuilder.DropIndex(
                name: "IX_Performances_TrackEventId",
                table: "Performances");

            migrationBuilder.DropColumn(
                name: "TrackEventId",
                table: "Performances");

            migrationBuilder.AddColumn<string>(
                name: "EventName",
                table: "Performances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Place",
                table: "Performances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "RunningEvent",
                table: "Performances",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
