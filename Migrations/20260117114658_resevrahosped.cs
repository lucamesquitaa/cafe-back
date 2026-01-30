using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turify.Migrations
{
    /// <inheritdoc />
    public partial class resevrahosped : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuartoReservas_Hotel_DetalhesModelId",
                table: "QuartoReservas");

            migrationBuilder.DropIndex(
                name: "IX_QuartoReservas_DetalhesModelId",
                table: "QuartoReservas");

            migrationBuilder.DropColumn(
                name: "CPF",
                table: "QuartoReservas");

            migrationBuilder.DropColumn(
                name: "DateBirth",
                table: "QuartoReservas");

            migrationBuilder.DropColumn(
                name: "DetalhesModelId",
                table: "QuartoReservas");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "QuartoReservas");

            migrationBuilder.DropColumn(
                name: "FamilyName",
                table: "QuartoReservas");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "QuartoReservas");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "QuartoReservas");

            migrationBuilder.DropColumn(
                name: "PriceDay",
                table: "QuartoReservas");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "QuartoReservas");

            migrationBuilder.RenameColumn(
                name: "SubStatus",
                table: "QuartoReservas",
                newName: "ReservaStatus");

            migrationBuilder.CreateTable(
                name: "Hospedes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReservationId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReservationId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    FamilyName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    CPF = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    DateBirth = table.Column<string>(type: "text", nullable: false),
                    CEP = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Complement = table.Column<string>(type: "text", nullable: true),
                    BloodType = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hospedes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hospedes_QuartoReservas_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "QuartoReservas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Hospedes_QuartoReservas_ReservationId1",
                        column: x => x.ReservationId1,
                        principalTable: "QuartoReservas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Hospedes_ReservationId",
                table: "Hospedes",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Hospedes_ReservationId1",
                table: "Hospedes",
                column: "ReservationId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Hospedes");

            migrationBuilder.RenameColumn(
                name: "ReservaStatus",
                table: "QuartoReservas",
                newName: "SubStatus");

            migrationBuilder.AddColumn<string>(
                name: "CPF",
                table: "QuartoReservas",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DateBirth",
                table: "QuartoReservas",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "DetalhesModelId",
                table: "QuartoReservas",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "QuartoReservas",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FamilyName",
                table: "QuartoReservas",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "QuartoReservas",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "QuartoReservas",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "PriceDay",
                table: "QuartoReservas",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "QuartoReservas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_QuartoReservas_DetalhesModelId",
                table: "QuartoReservas",
                column: "DetalhesModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuartoReservas_Hotel_DetalhesModelId",
                table: "QuartoReservas",
                column: "DetalhesModelId",
                principalTable: "Hotel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
