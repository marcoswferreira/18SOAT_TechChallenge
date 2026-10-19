using Domain.Entities;

namespace Application.Services.Notifications;

public interface IOrdemServicoNotificationService
{
    Task NotificarOrdemServicoCriadaAsync(OrdemServico os, Cliente? cliente = null, Veiculo? veiculo = null, CancellationToken cancellationToken = default);
    Task NotificarOrcamentoParaAprovacaoAsync(OrdemServico os, Cliente? cliente = null, Veiculo? veiculo = null, CancellationToken cancellationToken = default);
    Task NotificarOrcamentoAprovadoAsync(OrdemServico os, Cliente? cliente = null, Veiculo? veiculo = null, CancellationToken cancellationToken = default);
    Task NotificarOrcamentoRejeitadoAsync(OrdemServico os, Cliente? cliente = null, Veiculo? veiculo = null, CancellationToken cancellationToken = default);
    Task NotificarServicoFinalizadoAsync(OrdemServico os, Cliente? cliente = null, Veiculo? veiculo = null, CancellationToken cancellationToken = default);
    Task NotificarVeiculoEntregueAsync(OrdemServico os, Cliente? cliente = null, Veiculo? veiculo = null, CancellationToken cancellationToken = default);
}

