using Application.Services.Notifications;
using Application.UseCases.OrdensServico.Dto;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.OrdensServico;

public class RejeitarOrcamentoUseCase(
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

        os.RejeitarOrcamento();

        // Se o orçamento foi cancelado, libera a reserva das peças no estoque
        foreach (var peca in os.Pecas)
        {
            var itemEstoque = await _itemEstoqueRepository.GetByIdAsync(peca.ItemEstoqueId, cancellationToken);
            if (itemEstoque != null && itemEstoque.QuantidadeReservada >= peca.Quantidade)
            {
                itemEstoque.LiberarReserva(peca.Quantidade);
                _itemEstoqueRepository.Update(itemEstoque);
            }
        }

        _ordemServicoRepository.Update(os);
        await _ordemServicoRepository.SaveChangesAsync(cancellationToken);

        await _notificationService.NotificarOrcamentoRejeitadoAsync(os, null, null, cancellationToken);

        return OrdemServicoOutput.FromEntity(os);
    }
}

