using Domain.Common.Paginate;
using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IClienteRepository : IBaseRepository<Cliente>
{
    Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Cliente?> GetByIdWithVeiculosAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Cliente?> GetByDocumentoAsync(string documento, CancellationToken cancellationToken = default);
    Task<bool> ExistsByDocumentoAsync(string documento, CancellationToken cancellationToken = default);
    Task<IPaginate<Cliente>> GetPagedAsync(int index, int size, CancellationToken cancellationToken = default);
}