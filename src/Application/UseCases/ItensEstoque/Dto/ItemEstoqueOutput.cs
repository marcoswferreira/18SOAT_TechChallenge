using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases.ItensEstoque.Dto;

public record ItemEstoqueOutput(
    Guid Id,
    string Codigo,
    string Nome,
    string? Descricao,
    UnidadeMedida UnidadeMedida,
    decimal PrecoUnitario,
    decimal QuantidadeEmEstoque,
    decimal QuantidadeReservada,
    decimal QuantidadeDisponivel,
    DateTime CreatedAt,
    DateTime? LastModifiedAt)
{
    public static ItemEstoqueOutput FromEntity(ItemEstoque item) => new(
        item.Id,
        item.Codigo,
        item.Nome,
        item.Descricao,
        item.UnidadeMedida,
        item.PrecoUnitario,
        item.QuantidadeEmEstoque,
        item.QuantidadeReservada,
        item.QuantidadeDisponivel,
        item.CreatedAt,
        item.LastModifiedAt);
}
