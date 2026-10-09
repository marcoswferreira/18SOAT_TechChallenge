using Domain.Common.Paginate;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Database.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class ClienteRepository(ApplicationDbContext context) : BaseRepository<Cliente>(context), IClienteRepository
{
    public Task AddAsync(Cliente cliente, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Cliente cliente, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Cliente?> GetByCPFCNPJAsync(string CPFCNPJ, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Cliente?> GetByRefreshTokenAsync(string refreshTokenHash, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IPaginate<Cliente>> GetPagedAsync(int index, int size, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Cliente cliente, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}