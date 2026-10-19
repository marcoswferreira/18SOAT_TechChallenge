using Domain.Common.Paginate;
using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Interfaces.Repositories;

public interface IOrdemServicoRepository : IBaseRepository<OrdemServico>
{
    Task<OrdemServico?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OrdemServico?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrdemServico>> GetByDocumentoClienteAsync(Documento documento, CancellationToken cancellationToken = default);
    Task<IPaginate<OrdemServico>> GetPagedAsync(int index, int size, CancellationToken cancellationToken = default);
}