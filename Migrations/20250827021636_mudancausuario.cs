using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaudeIA.Migrations
{
    /// <inheritdoc />
    public partial class mudancausuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Usuarios",
                newName: "LastName");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Usuarios",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Usuarios",
                newName: "FullName");
        }
    }
}
