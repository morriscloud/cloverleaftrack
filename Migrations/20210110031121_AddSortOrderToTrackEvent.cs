using Microsoft.EntityFrameworkCore.Migrations;

namespace CloverleafTrack.Migrations
{
    public partial class AddSortOrderToTrackEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "TrackEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "TrackEvents");
        }
    }
}
