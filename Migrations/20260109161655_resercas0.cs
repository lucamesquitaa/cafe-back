using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turify.Migrations
{
    /// <inheritdoc />
    public partial class resercas0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "QuartoAvailable",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReservationId",
                table: "QuartoAvailable",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isAvailable",
                table: "QuartoAvailable",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                table: "QuartoAvailable");

            migrationBuilder.DropColumn(
                name: "ReservationId",
                table: "QuartoAvailable");

            migrationBuilder.DropColumn(
                name: "isAvailable",
                table: "QuartoAvailable");
        }
    }
}
