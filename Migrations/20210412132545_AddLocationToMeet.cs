using Microsoft.EntityFrameworkCore.Migrations;

namespace CloverleafTrack.Migrations
{
    public partial class AddLocationToMeet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Meets",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Meets");
        }
    }
}
