using Domain.Common.Paginate;
using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IClienteRepository : IBaseRepository<Cliente>
{
    Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Cliente?> GetByIdWithVeiculosAsync(Guid id, CancellationToken cancellationToken);
    Task<Cliente?> GetByDocumentoAsync(string documento, CancellationToken cancellationToken);
    Task<bool> ExistsByDocumentoAsync(string documento, CancellationToken cancellationToken);
    Task<IPaginate<Cliente>> GetPagedAsync(int index, int size, CancellationToken cancellationToken);
}