using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turify.Migrations
{
    /// <inheritdoc />
    public partial class corr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryQuarto_Quartos_QuartosModelId",
                table: "CategoryQuarto");

            migrationBuilder.DropIndex(
                name: "IX_CategoryQuarto_QuartosModelId",
                table: "CategoryQuarto");

            migrationBuilder.DropColumn(
                name: "QuartosModelId",
                table: "CategoryQuarto");

            migrationBuilder.CreateTable(
                name: "QuartoCategory",
                columns: table => new
                {
                    CategoryQuartoId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuartosModelId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuartoCategory", x => new { x.CategoryQuartoId, x.QuartosModelId });
                    table.ForeignKey(
                        name: "FK_QuartoCategory_CategoryQuarto_CategoryQuartoId",
                        column: x => x.CategoryQuartoId,
                        principalTable: "CategoryQuarto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuartoCategory_Quartos_QuartosModelId",
                        column: x => x.QuartosModelId,
                        principalTable: "Quartos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuartoCategory_QuartosModelId",
                table: "QuartoCategory",
                column: "QuartosModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuartoCategory");

            migrationBuilder.AddColumn<Guid>(
                name: "QuartosModelId",
                table: "CategoryQuarto",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoryQuarto_QuartosModelId",
                table: "CategoryQuarto",
                column: "QuartosModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryQuarto_Quartos_QuartosModelId",
                table: "CategoryQuarto",
                column: "QuartosModelId",
                principalTable: "Quartos",
                principalColumn: "Id");
        }
    }
}
