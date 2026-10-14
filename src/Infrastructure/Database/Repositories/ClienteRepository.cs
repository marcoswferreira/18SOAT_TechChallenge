using Domain.Common.Paginate;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;
using Infrastructure.Database.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class ClienteRepository(ApplicationDbContext context) : BaseRepository<Cliente>(context), IClienteRepository
{
    public async Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<Cliente?> GetByIdWithVeiculosAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(u => u.Veiculos)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<Cliente?> GetByDocumentoAsync(string documento, CancellationToken cancellationToken)
    {
        var docVO = new Documento(documento);
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Documento == docVO, cancellationToken);
    }

    public async Task<bool> ExistsByDocumentoAsync(string documento, CancellationToken cancellationToken)
    {
        var docVO = new Documento(documento);
        return await _dbSet
            .AnyAsync(u => u.Documento == docVO, cancellationToken);
    }

    public async Task<IPaginate<Cliente>> GetPagedAsync(int index, int size, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .OrderBy(u => u.Nome)
            .ToPaginateAsync(index, size, 0, cancellationToken);
    }
}