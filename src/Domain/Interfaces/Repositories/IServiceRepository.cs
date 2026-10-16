using Domain.Common.Paginate;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces.Repositories
{
    public interface IServiceRepository : IBaseRepository<Service>
    {
        Task<Service?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Service?> GetByServiceNameAsync(string serviceName, CancellationToken cancellationToken);
        Task<IPaginate<Service>> GetPagedAsync(int index, int size, CancellationToken cancellationToken);
        Task AddSync(Service service, CancellationToken cancellationToken);
        Task UpdateSync(Service service, CancellationToken cancellationToken);
        Task DeleteSync(Service service, CancellationToken cancellationToken);
    }
}
