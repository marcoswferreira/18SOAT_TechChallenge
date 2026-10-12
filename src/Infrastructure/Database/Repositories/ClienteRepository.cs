using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Database.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class ClienteRepository(ApplicationDbContext context) : BaseRepository<Cliente>(context), IClienteRepository
{
    public async Task<bool> ExistsByCPFCNPJAsync(string cpfcnpj, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AnyAsync(u => u.CPFCNPJ == cpfcnpj, cancellationToken);
    }

    public async Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
}