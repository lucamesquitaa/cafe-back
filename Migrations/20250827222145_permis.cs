using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaudeIA.Migrations
{
    /// <inheritdoc />
    public partial class permis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hotel_Usuarios_UserModelId",
                table: "Hotel");

            migrationBuilder.DropIndex(
                name: "IX_Hotel_UserModelId",
                table: "Hotel");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UserModelId",
                table: "Hotel");

            migrationBuilder.CreateTable(
                name: "UsuarioPermissao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    DetalhesModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioPermissao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioPermissao_Hotel_DetalhesModelId",
                        column: x => x.DetalhesModelId,
                        principalTable: "Hotel",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UsuarioPermissao_Usuarios_UserModelId",
                        column: x => x.UserModelId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPermissao_DetalhesModelId",
                table: "UsuarioPermissao",
                column: "DetalhesModelId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPermissao_UserModelId",
                table: "UsuarioPermissao",
                column: "UserModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuarioPermissao");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Usuarios",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UserModelId",
                table: "Hotel",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Hotel_UserModelId",
                table: "Hotel",
                column: "UserModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotel_Usuarios_UserModelId",
                table: "Hotel",
                column: "UserModelId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }
    }
}
