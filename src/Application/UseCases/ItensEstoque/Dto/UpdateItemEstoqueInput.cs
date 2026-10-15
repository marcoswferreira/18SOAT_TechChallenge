using Domain.Enums;

namespace Application.UseCases.ItensEstoque.Dto;

public record UpdateItemEstoqueInput(
    string Nome,
    string? Descricao,
    UnidadeMedida UnidadeMedida,
    decimal PrecoUnitario);
