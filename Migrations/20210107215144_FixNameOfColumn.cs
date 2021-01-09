using Microsoft.EntityFrameworkCore.Migrations;

namespace CloverleafTrack.Migrations
{
    public partial class FixNameOfColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FrationalInches",
                table: "Performances",
                newName: "FractionalInches");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FractionalInches",
                table: "Performances",
                newName: "FrationalInches");
        }
    }
}
