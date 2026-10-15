using Domain.Enums;

namespace Application.UseCases.ItensEstoque.Dto;

public record CreateItemEstoqueInput(
    string Codigo,
    string Nome,
    string? Descricao,
    UnidadeMedida UnidadeMedida,
    decimal PrecoUnitario);
