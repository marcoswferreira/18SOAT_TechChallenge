using Domain.Common.Paginate;
using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IClienteRepository : IBaseRepository<Cliente>
{
    Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Cliente?> GetByCPFCNPJAsync(string CPFCNPJ, CancellationToken cancellationToken);
    Task<Cliente?> GetByRefreshTokenAsync(string refreshTokenHash, CancellationToken cancellationToken);
    Task AddAsync(Cliente cliente, CancellationToken cancellationToken);
    Task UpdateAsync(Cliente cliente, CancellationToken cancellationToken);
    Task DeleteAsync(Cliente cliente, CancellationToken cancellationToken);
    Task<IPaginate<Cliente>> GetPagedAsync(int index, int size, CancellationToken cancellationToken);
}