using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Interfaces.Repositories;

public interface IOrdemServicoRepository
{
    Task<OrdemServico?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<OrdemServico>> ObterPorDocumentoClienteAsync(CpfCnpj documento, CancellationToken cancellationToken);
    Task SalvarAsync(OrdemServico ordemServico, CancellationToken cancellationToken);
}
