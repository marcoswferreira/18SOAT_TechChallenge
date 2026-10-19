namespace Application.UseCases.OrdensServico.Dto;

public record AdicionarPecaInput(Guid ItemEstoqueId, decimal Quantidade, decimal? ValorUnitario = null);

