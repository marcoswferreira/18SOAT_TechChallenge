using Application.Services.Notifications;
using Application.UseCases.OrdensServico.Dto;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.OrdensServico;

public class AprovarOrcamentoUseCase(
    IOrdemServicoRepository repository,
    IOrdemServicoNotificationService notificationService)
{
    private readonly IOrdemServicoRepository _repository = repository;
    private readonly IOrdemServicoNotificationService _notificationService = notificationService;

    public async Task<OrdemServicoOutput> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var os = await _repository.GetByIdWithDetailsAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Ordem de Serviço com ID '{id}' não foi encontrada.");

        os.AprovarOrcamento();
        os.ExecutarServico();

        _repository.Update(os);
        await _repository.SaveChangesAsync(cancellationToken);

        await _notificationService.NotificarOrcamentoAprovadoAsync(os, null, null, cancellationToken);

        return OrdemServicoOutput.FromEntity(os);
    }
}

