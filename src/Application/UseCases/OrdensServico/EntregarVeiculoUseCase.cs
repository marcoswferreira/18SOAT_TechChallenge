using Application.UseCases.OrdensServico.Dto;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;

namespace Application.UseCases.OrdensServico;

public class EntregarVeiculoUseCase(
    IOrdemServicoRepository repository,
    IOrdemServicoNotificationService notificationService)
{
    private readonly IOrdemServicoRepository _repository = repository;
    private readonly IOrdemServicoNotificationService _notificationService = notificationService;

    public async Task<OrdemServicoOutput> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var os = await _repository.GetByIdWithDetailsAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Ordem de Serviço com ID '{id}' não foi encontrada.");

        os.RegistrarEntrega();

        _repository.Update(os);
        await _repository.SaveChangesAsync(cancellationToken);

        await _notificationService.NotificarVeiculoEntregueAsync(os, null, null, cancellationToken);

        return OrdemServicoOutput.FromEntity(os);
    }
}

