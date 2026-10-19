using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrdemServicoItensEstoqueEServicos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ServicoId",
                table: "OrdensServico_Servicos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantidade",
                table: "OrdensServico_Pecas",
                type: "numeric(12,3)",
                precision: 12,
                scale: 3,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<Guid>(
                name: "ItemEstoqueId",
                table: "OrdensServico_Pecas",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServicoId",
                table: "OrdensServico_Servicos");

            migrationBuilder.DropColumn(
                name: "ItemEstoqueId",
                table: "OrdensServico_Pecas");

            migrationBuilder.AlterColumn<int>(
                name: "Quantidade",
                table: "OrdensServico_Pecas",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,3)",
                oldPrecision: 12,
                oldScale: 3);
        }
    }
}
