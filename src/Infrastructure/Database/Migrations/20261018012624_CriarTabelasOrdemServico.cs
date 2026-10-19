using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class CriarTabelasOrdemServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrdensServico",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    VeiculoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdensServico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdensServico_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdensServico_Veiculos_VeiculoId",
                        column: x => x.VeiculoId,
                        principalTable: "Veiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdensServico_Historico",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrdemServicoId = table.Column<Guid>(type: "uuid", nullable: false),
                    De = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Para = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdensServico_Historico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdensServico_Historico_OrdensServico_OrdemServicoId",
                        column: x => x.OrdemServicoId,
                        principalTable: "OrdensServico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdensServico_Pecas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomePeca = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    OrdemServicoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdensServico_Pecas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdensServico_Pecas_OrdensServico_OrdemServicoId",
                        column: x => x.OrdemServicoId,
                        principalTable: "OrdensServico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdensServico_Servicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    OrdemServicoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdensServico_Servicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdensServico_Servicos_OrdensServico_OrdemServicoId",
                        column: x => x.OrdemServicoId,
                        principalTable: "OrdensServico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_ClienteId",
                table: "OrdensServico",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_Status",
                table: "OrdensServico",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_VeiculoId",
                table: "OrdensServico",
                column: "VeiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_Historico_OrdemServicoId_Para",
                table: "OrdensServico_Historico",
                columns: new[] { "OrdemServicoId", "Para" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_Pecas_OrdemServicoId",
                table: "OrdensServico_Pecas",
                column: "OrdemServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_Servicos_OrdemServicoId",
                table: "OrdensServico_Servicos",
                column: "OrdemServicoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdensServico_Historico");

            migrationBuilder.DropTable(
                name: "OrdensServico_Pecas");

            migrationBuilder.DropTable(
                name: "OrdensServico_Servicos");

            migrationBuilder.DropTable(
                name: "OrdensServico");
        }
    }
}
