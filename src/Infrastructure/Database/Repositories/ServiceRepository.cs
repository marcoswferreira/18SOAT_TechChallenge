using Infrastructure.Database.DbContexts;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Common.Paginate;

namespace Infrastructure.Database.Repositories
{
    public class ServiceRepository : BaseRepository<Service>, IServiceRepository
    {
        public ServiceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddSync(Service service, CancellationToken cancellationToken)
        {
            await InsertAsync(service, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateSync(Service service, CancellationToken cancellationToken)
        {
            Update(service);
            await SaveChangesAsync(cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteSync(Service service, CancellationToken cancellationToken)
        {
            Delete(service);
            await SaveChangesAsync(cancellationToken);
        }


        public async Task<Service?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.IdService == id, cancellationToken);
        }

        public async Task<Service?> GetByServiceNameAsync(string serviceName, CancellationToken cancellationToken)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.ServiceName == serviceName, cancellationToken);
        }

        public async Task<IPaginate<Service>> GetPagedAsync(int index, int size, CancellationToken cancellationToken)
        {
            return await _dbSet
                            .AsNoTracking()
                            .OrderBy(s => s.ServiceName)
                            .ToPaginateAsync(index, size, 0, cancellationToken);
        }

    }
}
