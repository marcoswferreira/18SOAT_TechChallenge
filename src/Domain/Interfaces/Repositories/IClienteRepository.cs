using Domain.Common.Paginate;
using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IClienteRepository : IBaseRepository<Cliente>
{
    Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    //Task<Cliente?> GetByCPFCNPJAsync(string cpfcnpj, CancellationToken cancellationToken);
    Task<bool> ExistsByCPFCNPJAsync(string cpfcnpj, CancellationToken cancellationToken);
}