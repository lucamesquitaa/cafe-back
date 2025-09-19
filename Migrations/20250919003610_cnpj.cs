using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaudeIA.Migrations
{
    /// <inheritdoc />
    public partial class cnpj : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioPermissao_Hotel_DetalhesModelId",
                table: "UsuarioPermissao");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioPermissao_Usuarios_UserModelId",
                table: "UsuarioPermissao");

            migrationBuilder.AddColumn<string>(
                name: "Cnpj",
                table: "Hotel",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CpfRep",
                table: "Hotel",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmailRep",
                table: "Hotel",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomeRep",
                table: "Hotel",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Razao",
                table: "Hotel",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TelRep",
                table: "Hotel",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioPermissao_Hotel_DetalhesModelId",
                table: "UsuarioPermissao",
                column: "DetalhesModelId",
                principalTable: "Hotel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioPermissao_Usuarios_UserModelId",
                table: "UsuarioPermissao",
                column: "UserModelId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioPermissao_Hotel_DetalhesModelId",
                table: "UsuarioPermissao");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioPermissao_Usuarios_UserModelId",
                table: "UsuarioPermissao");

            migrationBuilder.DropColumn(
                name: "Cnpj",
                table: "Hotel");

            migrationBuilder.DropColumn(
                name: "CpfRep",
                table: "Hotel");

            migrationBuilder.DropColumn(
                name: "EmailRep",
                table: "Hotel");

            migrationBuilder.DropColumn(
                name: "NomeRep",
                table: "Hotel");

            migrationBuilder.DropColumn(
                name: "Razao",
                table: "Hotel");

            migrationBuilder.DropColumn(
                name: "TelRep",
                table: "Hotel");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioPermissao_Hotel_DetalhesModelId",
                table: "UsuarioPermissao",
                column: "DetalhesModelId",
                principalTable: "Hotel",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioPermissao_Usuarios_UserModelId",
                table: "UsuarioPermissao",
                column: "UserModelId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }
    }
}
