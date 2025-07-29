using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrandHotelPetrichMVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class AmenityIsActiveFieldAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Amenities",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Amenities");
        }
    }
}
