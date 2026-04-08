using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Turify.Migrations
{
    /// <inheritdoc />
    public partial class InitialReset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ErrorLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Message = table.Column<string>(type: "text", nullable: false),
                    StackTrace = table.Column<string>(type: "text", nullable: true),
                    Path = table.Column<string>(type: "text", nullable: false),
                    Method = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorLogs", x => x.Id);
                });

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
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                name: "CategoryQuarto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HotelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: true),
                    MinHospedes = table.Column<int>(type: "integer", nullable: true),
                    MaxHospedes = table.Column<int>(type: "integer", nullable: true),
                    AceitaCamaExtra = table.Column<bool>(type: "boolean", nullable: false),
                    AceitaBerco = table.Column<bool>(type: "boolean", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryQuarto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryQuarto_Hotel_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HotelId = table.Column<Guid>(type: "uuid", nullable: false),
                    DetalhesId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Contact = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DetalhesModelId = table.Column<Guid>(type: "uuid", nullable: false)
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
                        name: "FK_Contacts_Hotel_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quartos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HotelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Numero = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    MaxOcupation = table.Column<int>(type: "integer", nullable: false),
                    Refund = table.Column<bool>(type: "boolean", nullable: true),
                    AreaSize = table.Column<string>(type: "text", nullable: true),
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
                    TypeTv = table.Column<string>(type: "text", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quartos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quartos_Hotel_HotelId",
                        column: x => x.HotelId,
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
                    HotelId = table.Column<Guid>(type: "uuid", nullable: false),
                    DetalhesId = table.Column<Guid>(type: "uuid", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: false),
                    DetalhesModelId = table.Column<Guid>(type: "uuid", nullable: false),
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
                        name: "FK_UsuarioPermissao_Hotel_HotelId",
                        column: x => x.HotelId,
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BedType = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    CategoryQuartoId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuartosModelId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BedsDTO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BedsDTO_CategoryQuarto_CategoryQuartoId",
                        column: x => x.CategoryQuartoId,
                        principalTable: "CategoryQuarto",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BedsDTO_Quartos_QuartosModelId",
                        column: x => x.QuartosModelId,
                        principalTable: "Quartos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HotelId = table.Column<Guid>(type: "uuid", nullable: true),
                    DetalhesId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuartosModelId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuartosId = table.Column<Guid>(type: "uuid", nullable: true),
                    Alt = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Stared = table.Column<bool>(type: "boolean", nullable: true)
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
                        name: "FK_Photos_Hotel_HotelId",
                        column: x => x.HotelId,
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

            migrationBuilder.CreateTable(
                name: "QuartoAvailable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuartosModelId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuartosId = table.Column<Guid>(type: "uuid", nullable: true),
                    isAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    ReservationId = table.Column<int>(type: "integer", nullable: true),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "QuartoReservas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuartosModelId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuartosId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReservaStatus = table.Column<int>(type: "integer", nullable: false),
                    Checkin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Checkout = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EarlyCheckin = table.Column<bool>(type: "boolean", nullable: true),
                    LateCheckout = table.Column<bool>(type: "boolean", nullable: true),
                    Adults = table.Column<int>(type: "integer", nullable: false),
                    Kids = table.Column<int>(type: "integer", nullable: false),
                    Cupom = table.Column<string>(type: "text", nullable: true),
                    PriceTotal = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuartoReservas", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "Hospedes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReservationId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    BloodType = table.Column<string>(type: "text", nullable: true),
                    Principal = table.Column<bool>(type: "boolean", nullable: true),
                    TextArea = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hospedes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hospedes_QuartoReservas_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "QuartoReservas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BedsDTO_CategoryQuartoId",
                table: "BedsDTO",
                column: "CategoryQuartoId");

            migrationBuilder.CreateIndex(
                name: "IX_BedsDTO_QuartosModelId",
                table: "BedsDTO",
                column: "QuartosModelId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryQuarto_HotelId",
                table: "CategoryQuarto",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_DetalhesId",
                table: "Contacts",
                column: "DetalhesId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_HotelId",
                table: "Contacts",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Hospedes_ReservationId",
                table: "Hospedes",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_DetalhesId",
                table: "Photos",
                column: "DetalhesId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_HotelId",
                table: "Photos",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_QuartosId",
                table: "Photos",
                column: "QuartosId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_QuartosModelId",
                table: "Photos",
                column: "QuartosModelId");

            migrationBuilder.CreateIndex(
                name: "IX_QuartoAvailable_QuartosId",
                table: "QuartoAvailable",
                column: "QuartosId");

            migrationBuilder.CreateIndex(
                name: "IX_QuartoAvailable_QuartosModelId",
                table: "QuartoAvailable",
                column: "QuartosModelId");

            migrationBuilder.CreateIndex(
                name: "IX_QuartoCategory_QuartosModelId",
                table: "QuartoCategory",
                column: "QuartosModelId");

            migrationBuilder.CreateIndex(
                name: "IX_QuartoReservas_QuartosId",
                table: "QuartoReservas",
                column: "QuartosId");

            migrationBuilder.CreateIndex(
                name: "IX_QuartoReservas_QuartosModelId",
                table: "QuartoReservas",
                column: "QuartosModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Quartos_HotelId",
                table: "Quartos",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPermissao_DetalhesId",
                table: "UsuarioPermissao",
                column: "DetalhesId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPermissao_HotelId",
                table: "UsuarioPermissao",
                column: "HotelId");

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
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "ErrorLogs");

            migrationBuilder.DropTable(
                name: "Hospedes");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "QuartoAvailable");

            migrationBuilder.DropTable(
                name: "QuartoCategory");

            migrationBuilder.DropTable(
                name: "UsuarioPermissao");

            migrationBuilder.DropTable(
                name: "QuartoReservas");

            migrationBuilder.DropTable(
                name: "CategoryQuarto");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Quartos");

            migrationBuilder.DropTable(
                name: "Hotel");
        }
    }
}
