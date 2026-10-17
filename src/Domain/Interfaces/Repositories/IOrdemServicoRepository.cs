using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Interfaces.Repositories;

public interface IOrdemServicoRepository
{
    Task<OrdemServico?> ObterPorIdAsync(Guid id, CancellationToken ct = default);

    Task<IEnumerable<OrdemServico>> ObterPorDocumentoClienteAsync(
        Documento documento, CancellationToken ct = default);

    Task<IEnumerable<OrdemServico>> ListarTodasAsync(CancellationToken ct = default);

    Task AdicionarAsync(OrdemServico ordemServico, CancellationToken ct = default);
    Task SalvarAlteracoesAsync(CancellationToken ct = default);
}