using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turify.Migrations
{
    /// <inheritdoc />
    public partial class novocomquartos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hotel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Rede = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Child = table.Column<bool>(type: "boolean", nullable: true),
                    Pets = table.Column<bool>(type: "boolean", nullable: true),
                    PetsTax = table.Column<double>(type: "double precision", nullable: true),
                    Cep = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Number = table.Column<string>(type: "text", nullable: false),
                    Complement = table.Column<string>(type: "text", nullable: true),
                    Lobby = table.Column<string>(type: "text", nullable: true),
                    Diff = table.Column<string>(type: "text", nullable: true),
                    Beach = table.Column<bool>(type: "boolean", nullable: true),
                    Downtown = table.Column<bool>(type: "boolean", nullable: true),
                    Airpot = table.Column<bool>(type: "boolean", nullable: true),
                    Highway = table.Column<bool>(type: "boolean", nullable: true),
                    Hospital = table.Column<bool>(type: "boolean", nullable: true),
                    Coffee = table.Column<bool>(type: "boolean", nullable: true),
                    Wifi = table.Column<bool>(type: "boolean", nullable: true),
                    Swimming = table.Column<bool>(type: "boolean", nullable: true),
                    Cleaning = table.Column<bool>(type: "boolean", nullable: true),
                    Gym = table.Column<bool>(type: "boolean", nullable: true),
                    Cnpj = table.Column<string>(type: "text", nullable: false),
                    Razao = table.Column<string>(type: "text", nullable: false),
                    NomeRep = table.Column<string>(type: "text", nullable: false),
                    TelRep = table.Column<string>(type: "text", nullable: false),
                    CpfRep = table.Column<string>(type: "text", nullable: false),
                    EmailRep = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GoogleId = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Photo = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DetalhesModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    DetalhesId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Contact = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DetalhesModelId1 = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contacts_Hotel_DetalhesId",
                        column: x => x.DetalhesId,
                        principalTable: "Hotel",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contacts_Hotel_DetalhesModelId",
                        column: x => x.DetalhesModelId,
                        principalTable: "Hotel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quartos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DetalhesModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Tags = table.Column<string[]>(type: "text[]", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    MaxOcupation = table.Column<int>(type: "integer", nullable: false),
                    Refund = table.Column<bool>(type: "boolean", nullable: true),
                    AreaSize = table.Column<string>(type: "text", nullable: false),
                    Diff = table.Column<string>(type: "text", nullable: true),
                    Freeze = table.Column<bool>(type: "boolean", nullable: true),
                    Vault = table.Column<bool>(type: "boolean", nullable: true),
                    Telephone = table.Column<bool>(type: "boolean", nullable: true),
                    Coffee = table.Column<bool>(type: "boolean", nullable: true),
                    Wifi = table.Column<bool>(type: "boolean", nullable: true),
                    Fridge = table.Column<bool>(type: "boolean", nullable: true),
                    Cleaning = table.Column<bool>(type: "boolean", nullable: true),
                    Varanda = table.Column<bool>(type: "boolean", nullable: true),
                    Bathroom = table.Column<bool>(type: "boolean", nullable: true),
                    BathProducts = table.Column<string>(type: "text", nullable: true),
                    Tv = table.Column<bool>(type: "boolean", nullable: true),
                    TypeTv = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quartos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quartos_Hotel_DetalhesModelId",
                        column: x => x.DetalhesModelId,
                        principalTable: "Hotel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioPermissao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserModelEmail = table.Column<string>(type: "text", nullable: false),
                    DetalhesModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    DetalhesId = table.Column<Guid>(type: "uuid", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: false),
                    DetalhesModelId1 = table.Column<Guid>(type: "uuid", nullable: false),
                    UserModelId1 = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioPermissao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioPermissao_Hotel_DetalhesId",
                        column: x => x.DetalhesId,
                        principalTable: "Hotel",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UsuarioPermissao_Hotel_DetalhesModelId",
                        column: x => x.DetalhesModelId,
                        principalTable: "Hotel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioPermissao_Usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UsuarioPermissao_Usuarios_UserModelId",
                        column: x => x.UserModelId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BedsDTO",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    QuartosModelId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BedsDTO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BedsDTO_Quartos_QuartosModelId",
                        column: x => x.QuartosModelId,
                        principalTable: "Quartos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CategoryQuarto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DetalhesModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: true),
                    QuartosModelId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryQuarto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryQuarto_Hotel_DetalhesModelId",
                        column: x => x.DetalhesModelId,
                        principalTable: "Hotel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryQuarto_Quartos_QuartosModelId",
                        column: x => x.QuartosModelId,
                        principalTable: "Quartos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DetalhesModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    DetalhesId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuartosModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuartosId = table.Column<Guid>(type: "uuid", nullable: true),
                    Alt = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Stared = table.Column<bool>(type: "boolean", nullable: true),
                    DetalhesModelId1 = table.Column<Guid>(type: "uuid", nullable: false),
                    QuartosModelId1 = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_Hotel_DetalhesId",
                        column: x => x.DetalhesId,
                        principalTable: "Hotel",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Photos_Hotel_DetalhesModelId",
                        column: x => x.DetalhesModelId,
                        principalTable: "Hotel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Photos_Quartos_QuartosId",
                        column: x => x.QuartosId,
                        principalTable: "Quartos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Photos_Quartos_QuartosModelId",
                        column: x => x.QuartosModelId,
                        principalTable: "Quartos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BedsDTO_QuartosModelId",
                table: "BedsDTO",
                column: "QuartosModelId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryQuarto_DetalhesModelId",
                table: "CategoryQuarto",
                column: "DetalhesModelId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryQuarto_QuartosModelId",
                table: "CategoryQuarto",
                column: "QuartosModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_DetalhesId",
                table: "Contacts",
                column: "DetalhesId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_DetalhesModelId",
                table: "Contacts",
                column: "DetalhesModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_DetalhesId",
                table: "Photos",
                column: "DetalhesId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_DetalhesModelId",
                table: "Photos",
                column: "DetalhesModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_QuartosId",
                table: "Photos",
                column: "QuartosId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_QuartosModelId",
                table: "Photos",
                column: "QuartosModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Quartos_DetalhesModelId",
                table: "Quartos",
                column: "DetalhesModelId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPermissao_DetalhesId",
                table: "UsuarioPermissao",
                column: "DetalhesId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPermissao_DetalhesModelId",
                table: "UsuarioPermissao",
                column: "DetalhesModelId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPermissao_UserId",
                table: "UsuarioPermissao",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPermissao_UserModelId",
                table: "UsuarioPermissao",
                column: "UserModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BedsDTO");

            migrationBuilder.DropTable(
                name: "CategoryQuarto");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "UsuarioPermissao");

            migrationBuilder.DropTable(
                name: "Quartos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Hotel");
        }
    }
}
