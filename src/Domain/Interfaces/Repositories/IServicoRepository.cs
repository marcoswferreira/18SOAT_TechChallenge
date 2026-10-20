using Domain.Common.Paginate;
using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IServicoRepository : IBaseRepository<Servico>
{
    Task<Servico?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Servico?> GetByServiceNameAsync(string serviceName, CancellationToken cancellationToken);
    Task<IPaginate<Servico>> GetPagedAsync(int index, int size, CancellationToken cancellationToken);
    Task AddSync(Servico service, CancellationToken cancellationToken);
    Task UpdateSync(Servico service, CancellationToken cancellationToken);
    Task DeleteSync(Servico service, CancellationToken cancellationToken);
}
