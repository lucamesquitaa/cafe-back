using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turify.Migrations
{
    /// <inheritdoc />
    public partial class earlyheckin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EarlyCheckin",
                table: "QuartoReservas",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LateCheckout",
                table: "QuartoReservas",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EarlyCheckin",
                table: "QuartoReservas");

            migrationBuilder.DropColumn(
                name: "LateCheckout",
                table: "QuartoReservas");
        }
    }
}
