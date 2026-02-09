using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turify.Migrations
{
    /// <inheritdoc />
    public partial class hospedeprincipal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hospedes_QuartoReservas_ReservationId",
                table: "Hospedes");

            migrationBuilder.DropForeignKey(
                name: "FK_Hospedes_QuartoReservas_ReservationId1",
                table: "Hospedes");

            migrationBuilder.DropIndex(
                name: "IX_Hospedes_ReservationId1",
                table: "Hospedes");

            migrationBuilder.DropColumn(
                name: "ReservationId1",
                table: "Hospedes");

            migrationBuilder.AddColumn<bool>(
                name: "Principal",
                table: "Hospedes",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextArea",
                table: "Hospedes",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Hospedes_QuartoReservas_ReservationId",
                table: "Hospedes",
                column: "ReservationId",
                principalTable: "QuartoReservas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hospedes_QuartoReservas_ReservationId",
                table: "Hospedes");

            migrationBuilder.DropColumn(
                name: "Principal",
                table: "Hospedes");

            migrationBuilder.DropColumn(
                name: "TextArea",
                table: "Hospedes");

            migrationBuilder.AddColumn<Guid>(
                name: "ReservationId1",
                table: "Hospedes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hospedes_ReservationId1",
                table: "Hospedes",
                column: "ReservationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Hospedes_QuartoReservas_ReservationId",
                table: "Hospedes",
                column: "ReservationId",
                principalTable: "QuartoReservas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Hospedes_QuartoReservas_ReservationId1",
                table: "Hospedes",
                column: "ReservationId1",
                principalTable: "QuartoReservas",
                principalColumn: "Id");
        }
    }
}
