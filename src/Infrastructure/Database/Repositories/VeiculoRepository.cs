using Domain.Common.Paginate;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;
using Infrastructure.Database.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class VeiculoRepository(ApplicationDbContext context) : BaseRepository<Veiculo>(context), IVeiculoRepository
{
    public async Task<Veiculo?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(v => v.Cliente)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<Veiculo?> GetByPlacaAsync(string placa, CancellationToken cancellationToken)
    {
        var placaVO = new Placa(placa);
        return await _dbSet
            .Include(v => v.Cliente)
            .FirstOrDefaultAsync(u => u.Placa == placaVO, cancellationToken);
    }

    public async Task<bool> ExistsByPlacaAsync(string placa, CancellationToken cancellationToken)
    {
        var placaVO = new Placa(placa);
        return await _dbSet
            .AnyAsync(u => u.Placa == placaVO, cancellationToken);
    }

    public async Task<IEnumerable<Veiculo>> GetByClienteIdAsync(Guid clienteId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Where(u => u.ClienteId == clienteId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IPaginate<Veiculo>> GetPagedAsync(int index, int size, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(v => v.Cliente)
            .AsNoTracking()
            .OrderBy(u => u.Marca)
            .ThenBy(u => u.Modelo)
            .ToPaginateAsync(index, size, 0, cancellationToken);
    }
}