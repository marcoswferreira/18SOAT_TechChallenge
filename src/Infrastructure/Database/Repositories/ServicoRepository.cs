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
    public class ServicoRepository : BaseRepository<Servico>, IServicoRepository
    {
        public ServicoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddSync(Servico service, CancellationToken cancellationToken)
        {
            await InsertAsync(service, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateSync(Servico service, CancellationToken cancellationToken)
        {
            Update(service);
            await SaveChangesAsync(cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteSync(Servico service, CancellationToken cancellationToken)
        {
            Delete(service);
            await SaveChangesAsync(cancellationToken);
        }


        public async Task<Servico?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await Query().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<Servico?> GetByServiceNameAsync(string serviceName, CancellationToken cancellationToken)
        {
            return await Query().FirstOrDefaultAsync(s => s.Nome == serviceName, cancellationToken);
        }

        public async Task<IPaginate<Servico>> GetPagedAsync(int index, int size, CancellationToken cancellationToken)
        {
            var query = Query(
                                orderBy: q => q.OrderBy(s => s.Nome),
                                enableTracking: false
                            );

            return await query.ToPaginateAsync(index, size, 0, cancellationToken);
        }

    }
}
