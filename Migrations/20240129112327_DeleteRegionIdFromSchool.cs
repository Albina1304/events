using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace events.Migrations
{
    /// <inheritdoc />
    public partial class DeleteRegionIdFromSchool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "Schools");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "Schools",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
