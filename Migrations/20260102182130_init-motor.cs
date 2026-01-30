using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turify.Migrations
{
    /// <inheritdoc />
    public partial class initmotor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuartoAvailable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuartosModelId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuartosId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DayPrice = table.Column<double>(type: "double precision", nullable: false),
                    MinDays = table.Column<int>(type: "integer", nullable: false),
                    MaxDays = table.Column<int>(type: "integer", nullable: false),
                    Reembolsavel = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuartoAvailable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuartoAvailable_Quartos_QuartosId",
                        column: x => x.QuartosId,
                        principalTable: "Quartos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuartoAvailable_Quartos_QuartosModelId",
                        column: x => x.QuartosModelId,
                        principalTable: "Quartos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuartoReservas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DetalhesModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuartosModelId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuartosId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SubStatus = table.Column<int>(type: "integer", nullable: false),
                    Checkin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Checkout = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Adults = table.Column<int>(type: "integer", nullable: false),
                    Kids = table.Column<int>(type: "integer", nullable: false),
                    Cupom = table.Column<string>(type: "text", nullable: true),
                    PriceDay = table.Column<double>(type: "double precision", nullable: false),
                    PriceTotal = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    FamilyName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    CPF = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    DateBirth = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuartoReservas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuartoReservas_Hotel_DetalhesModelId",
                        column: x => x.DetalhesModelId,
                        principalTable: "Hotel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuartoReservas_Quartos_QuartosId",
                        column: x => x.QuartosId,
                        principalTable: "Quartos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuartoReservas_Quartos_QuartosModelId",
                        column: x => x.QuartosModelId,
                        principalTable: "Quartos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuartoAvailable_QuartosId",
                table: "QuartoAvailable",
                column: "QuartosId");

            migrationBuilder.CreateIndex(
                name: "IX_QuartoAvailable_QuartosModelId",
                table: "QuartoAvailable",
                column: "QuartosModelId");

            migrationBuilder.CreateIndex(
                name: "IX_QuartoReservas_DetalhesModelId",
                table: "QuartoReservas",
                column: "DetalhesModelId");

            migrationBuilder.CreateIndex(
                name: "IX_QuartoReservas_QuartosId",
                table: "QuartoReservas",
                column: "QuartosId");

            migrationBuilder.CreateIndex(
                name: "IX_QuartoReservas_QuartosModelId",
                table: "QuartoReservas",
                column: "QuartosModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuartoAvailable");

            migrationBuilder.DropTable(
                name: "QuartoReservas");
        }
    }
}
