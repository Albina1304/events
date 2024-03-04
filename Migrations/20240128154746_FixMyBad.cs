using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace events.Migrations
{
    /// <inheritdoc />
    public partial class FixMyBad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "Schools");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Schools",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "Schools",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "Schools");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Schools",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "Schools",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
