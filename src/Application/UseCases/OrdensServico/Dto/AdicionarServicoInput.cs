namespace Application.UseCases.OrdensServico.Dto;

public record AdicionarServicoInput(Guid? ServicoId, string? Descricao, decimal ValorUnitario, int Quantidade = 1);

