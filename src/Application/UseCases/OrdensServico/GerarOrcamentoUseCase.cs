using Application.Services.Notifications;
using Application.UseCases.OrdensServico.Dto;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.OrdensServico;

public class GerarOrcamentoUseCase(
    IOrdemServicoRepository repository,
    IOrdemServicoNotificationService notificationService)
{
    private readonly IOrdemServicoRepository _repository = repository;
    private readonly IOrdemServicoNotificationService _notificationService = notificationService;

    public async Task<OrdemServicoOutput> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var os = await _repository.GetByIdWithDetailsAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Ordem de Serviço com ID '{id}' não foi encontrada.");

        os.GerarEEnviarOrcamento();

        _repository.Update(os);
        await _repository.SaveChangesAsync(cancellationToken);

        await _notificationService.NotificarOrcamentoParaAprovacaoAsync(os, null, null, cancellationToken);

        return OrdemServicoOutput.FromEntity(os);
    }
}

