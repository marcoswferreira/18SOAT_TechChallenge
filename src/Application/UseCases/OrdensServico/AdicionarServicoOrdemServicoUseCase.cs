using Application.UseCases.OrdensServico.Dto;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;

namespace Application.UseCases.OrdensServico;

public class AdicionarServicoOrdemServicoUseCase(
    IOrdemServicoRepository ordemServicoRepository,
    IServicoRepository servicoRepository)
{
    private readonly IOrdemServicoRepository _ordemServicoRepository = ordemServicoRepository;
    private readonly IServicoRepository _servicoRepository = servicoRepository;

    public async Task<OrdemServicoOutput> ExecuteAsync(
        Guid ordemServicoId,
        AdicionarServicoInput input,
        CancellationToken cancellationToken = default)
    {
        var os = await _ordemServicoRepository.GetByIdWithDetailsAsync(ordemServicoId, cancellationToken)
            ?? throw new KeyNotFoundException($"Ordem de Serviço com ID '{ordemServicoId}' não foi encontrada.");

        if (input.ServicoId.HasValue)
        {
            var servico = await _servicoRepository.GetByIdAsync(input.ServicoId.Value, cancellationToken)
                ?? throw new KeyNotFoundException($"Serviço com ID '{input.ServicoId.Value}' não foi encontrado.");

            os.AdicionarServico(servico, input.ValorUnitario, input.Quantidade);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(input.Descricao))
                throw new ArgumentException("A descrição do serviço ou o ID do serviço deve ser informado.", nameof(input.Descricao));

            os.AdicionarServico(ItemServico.Criar(input.Descricao, input.ValorUnitario, input.Quantidade));
        }

        _ordemServicoRepository.Update(os);
        await _ordemServicoRepository.SaveChangesAsync(cancellationToken);

        return OrdemServicoOutput.FromEntity(os);
    }
}

