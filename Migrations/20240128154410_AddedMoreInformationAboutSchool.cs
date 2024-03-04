using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace events.Migrations
{
    /// <inheritdoc />
    public partial class AddedMoreInformationAboutSchool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Schools",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "House",
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

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Schools",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "House",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Schools");
        }
    }
}
