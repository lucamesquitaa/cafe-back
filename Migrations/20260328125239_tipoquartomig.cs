using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turify.Migrations
{
    /// <inheritdoc />
    public partial class tipoquartomig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserModelEmail",
                table: "UsuarioPermissao");

            migrationBuilder.AddColumn<bool>(
                name: "AceitaBerco",
                table: "CategoryQuarto",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AceitaCamaExtra",
                table: "CategoryQuarto",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CategoryQuarto",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "CategoryQuarto",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxHospedes",
                table: "CategoryQuarto",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinHospedes",
                table: "CategoryQuarto",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryQuartoId",
                table: "BedsDTO",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BedsDTO_CategoryQuartoId",
                table: "BedsDTO",
                column: "CategoryQuartoId");

            migrationBuilder.AddForeignKey(
                name: "FK_BedsDTO_CategoryQuarto_CategoryQuartoId",
                table: "BedsDTO",
                column: "CategoryQuartoId",
                principalTable: "CategoryQuarto",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BedsDTO_CategoryQuarto_CategoryQuartoId",
                table: "BedsDTO");

            migrationBuilder.DropIndex(
                name: "IX_BedsDTO_CategoryQuartoId",
                table: "BedsDTO");

            migrationBuilder.DropColumn(
                name: "AceitaBerco",
                table: "CategoryQuarto");

            migrationBuilder.DropColumn(
                name: "AceitaCamaExtra",
                table: "CategoryQuarto");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CategoryQuarto");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "CategoryQuarto");

            migrationBuilder.DropColumn(
                name: "MaxHospedes",
                table: "CategoryQuarto");

            migrationBuilder.DropColumn(
                name: "MinHospedes",
                table: "CategoryQuarto");

            migrationBuilder.DropColumn(
                name: "CategoryQuartoId",
                table: "BedsDTO");

            migrationBuilder.AddColumn<string>(
                name: "UserModelEmail",
                table: "UsuarioPermissao",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
