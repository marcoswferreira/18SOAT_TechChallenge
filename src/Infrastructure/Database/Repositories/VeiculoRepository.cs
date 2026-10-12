using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Database.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class VeiculoRepository(ApplicationDbContext context) : BaseRepository<Veiculo>(context), IVeiculoRepository
{
    public async Task<bool> ExistsByPlacaAsync(string placa, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AnyAsync(u => u.Placa.Valor == placa, cancellationToken);
    }

    public async Task<Veiculo?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
}