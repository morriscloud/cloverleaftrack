using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloverleafTrack.Migrations
{
    public partial class AddFieldEventEnvironment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Environment",
                table: "FieldEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Environment",
                table: "FieldEvents");
        }
    }
}
