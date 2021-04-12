using Microsoft.EntityFrameworkCore.Migrations;

namespace CloverleafTrack.Migrations
{
    public partial class AddOutdoorFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Outdoor",
                table: "Meets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Outdoor",
                table: "Meets");
        }
    }
}
