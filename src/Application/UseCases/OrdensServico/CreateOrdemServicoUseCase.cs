using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Application.UseCases.OrdensServico.Dto;

namespace Application.UseCases.OrdensServico;

public class CreateOrdemServicoUseCase(
    IOrdemServicoRepository ordemServicoRepository,
    IClienteRepository clienteRepository,
    IVeiculoRepository veiculoRepository,
    IOrdemServicoNotificationService notificationService)
{
    private readonly IOrdemServicoRepository _ordemServicoRepository = ordemServicoRepository;
    private readonly IClienteRepository _clienteRepository = clienteRepository;
    private readonly IVeiculoRepository _veiculoRepository = veiculoRepository;
    private readonly IOrdemServicoNotificationService _notificationService = notificationService;

    public async Task<OrdemServicoOutput> ExecuteAsync(CreateOrdemServicoInput input, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.GetByIdAsync(input.ClienteId, cancellationToken)
            ?? throw new KeyNotFoundException($"Cliente com ID '{input.ClienteId}' não foi encontrado.");

        var veiculo = await _veiculoRepository.GetByIdAsync(input.VeiculoId, cancellationToken)
            ?? throw new KeyNotFoundException($"Veículo com ID '{input.VeiculoId}' não foi encontrado.");

        var ordemServico = new OrdemServico(cliente.Id, veiculo.Id);

        await _ordemServicoRepository.InsertAsync(ordemServico, cancellationToken);
        await _ordemServicoRepository.SaveChangesAsync(cancellationToken);

        await _notificationService.NotificarOrdemServicoCriadaAsync(ordemServico, cliente, veiculo, cancellationToken);

        return OrdemServicoOutput.FromEntity(ordemServico);
    }
}

