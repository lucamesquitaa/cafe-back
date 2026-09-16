using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cafeteria.Migrations
{
    /// <inheritdoc />
    public partial class FixPermissoesRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioPermissao_Cafeterias_CafeteriaId1",
                table: "UsuarioPermissao");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioPermissao_Usuarios_UserId",
                table: "UsuarioPermissao");

            migrationBuilder.DropIndex(
                name: "IX_UsuarioPermissao_CafeteriaId1",
                table: "UsuarioPermissao");

            migrationBuilder.DropIndex(
                name: "IX_UsuarioPermissao_UserId",
                table: "UsuarioPermissao");

            migrationBuilder.DropColumn(
                name: "CafeteriaId1",
                table: "UsuarioPermissao");

            migrationBuilder.DropColumn(
                name: "CafeteriaId2",
                table: "UsuarioPermissao");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UsuarioPermissao");

            migrationBuilder.DropColumn(
                name: "UserModelId1",
                table: "UsuarioPermissao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CafeteriaId1",
                table: "UsuarioPermissao",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CafeteriaId2",
                table: "UsuarioPermissao",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "UsuarioPermissao",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserModelId1",
                table: "UsuarioPermissao",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPermissao_CafeteriaId1",
                table: "UsuarioPermissao",
                column: "CafeteriaId1");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPermissao_UserId",
                table: "UsuarioPermissao",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioPermissao_Cafeterias_CafeteriaId1",
                table: "UsuarioPermissao",
                column: "CafeteriaId1",
                principalTable: "Cafeterias",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioPermissao_Usuarios_UserId",
                table: "UsuarioPermissao",
                column: "UserId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }
    }
}
