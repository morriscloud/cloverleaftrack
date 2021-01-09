using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CloverleafTrack.Migrations
{
    public partial class CreateMeetResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloverleafPlace",
                table: "Meets");

            migrationBuilder.CreateTable(
                name: "MeetResult",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CloverleafPlace = table.Column<int>(type: "int", nullable: false),
                    MeetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeetResult_Meets_MeetId",
                        column: x => x.MeetId,
                        principalTable: "Meets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeetResult_MeetId",
                table: "MeetResult",
                column: "MeetId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeetResult");

            migrationBuilder.AddColumn<int>(
                name: "CloverleafPlace",
                table: "Meets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
