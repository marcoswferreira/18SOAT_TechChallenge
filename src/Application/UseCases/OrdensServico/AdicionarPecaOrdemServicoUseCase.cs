using Application.UseCases.OrdensServico.Dto;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.OrdensServico;

public class AdicionarPecaOrdemServicoUseCase(
    IOrdemServicoRepository ordemServicoRepository,
    IItemEstoqueRepository itemEstoqueRepository)
{
    private readonly IOrdemServicoRepository _ordemServicoRepository = ordemServicoRepository;
    private readonly IItemEstoqueRepository _itemEstoqueRepository = itemEstoqueRepository;

    public async Task<OrdemServicoOutput> ExecuteAsync(
        Guid ordemServicoId,
        AdicionarPecaInput input,
        CancellationToken cancellationToken = default)
    {
        var os = await _ordemServicoRepository.GetByIdWithDetailsAsync(ordemServicoId, cancellationToken)
            ?? throw new KeyNotFoundException($"Ordem de Serviço com ID '{ordemServicoId}' não foi encontrada.");

        var itemEstoque = await _itemEstoqueRepository.GetByIdAsync(input.ItemEstoqueId, cancellationToken)
            ?? throw new KeyNotFoundException($"Item de estoque com ID '{input.ItemEstoqueId}' não foi encontrado.");

        // Reserva o estoque para esta ordem de serviço
        itemEstoque.Reservar(input.Quantidade);

        // Adiciona a peça com snapshot de dados na OS
        os.AdicionarPeca(itemEstoque, input.Quantidade, input.ValorUnitario);

        _itemEstoqueRepository.Update(itemEstoque);
        _ordemServicoRepository.Update(os);

        await _ordemServicoRepository.SaveChangesAsync(cancellationToken);

        return OrdemServicoOutput.FromEntity(os);
    }
}

