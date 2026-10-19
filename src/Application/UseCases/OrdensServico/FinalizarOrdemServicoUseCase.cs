using Application.UseCases.OrdensServico.Dto;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;

namespace Application.UseCases.OrdensServico;

public class FinalizarOrdemServicoUseCase(
    IOrdemServicoRepository ordemServicoRepository,
    IItemEstoqueRepository itemEstoqueRepository,
    IOrdemServicoNotificationService notificationService)
{
    private readonly IOrdemServicoRepository _ordemServicoRepository = ordemServicoRepository;
    private readonly IItemEstoqueRepository _itemEstoqueRepository = itemEstoqueRepository;
    private readonly IOrdemServicoNotificationService _notificationService = notificationService;

    public async Task<OrdemServicoOutput> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var os = await _ordemServicoRepository.GetByIdWithDetailsAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Ordem de Serviço com ID '{id}' não foi encontrada.");

        os.FinalizarServico();

        // Ao finalizar a OS, baixa definitivamente do estoque físico e da reserva
        foreach (var peca in os.Pecas)
        {
            var itemEstoque = await _itemEstoqueRepository.GetByIdAsync(peca.ItemEstoqueId, cancellationToken);
            if (itemEstoque != null && itemEstoque.QuantidadeReservada >= peca.Quantidade)
            {
                itemEstoque.BaixarReserva(peca.Quantidade);
                _itemEstoqueRepository.Update(itemEstoque);
            }
        }

        _ordemServicoRepository.Update(os);
        await _ordemServicoRepository.SaveChangesAsync(cancellationToken);

        await _notificationService.NotificarServicoFinalizadoAsync(os, null, null, cancellationToken);

        return OrdemServicoOutput.FromEntity(os);
    }
}

