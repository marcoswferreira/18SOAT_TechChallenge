using Domain.Common.Paginate;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Database.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class ItemEstoqueRepository(ApplicationDbContext context) : BaseRepository<ItemEstoque>(context), IItemEstoqueRepository
{
    public async Task<ItemEstoque?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCodigoAsync(string codigo, CancellationToken cancellationToken)
    {
        var codigoNormalizado = codigo.Trim().ToUpperInvariant();

        // IgnoreQueryFilters pra não deixar reaproveitar o código de um item excluído
        return await _dbSet
            .IgnoreQueryFilters()
            .AnyAsync(p => p.Codigo == codigoNormalizado, cancellationToken);
    }

    public async Task<IPaginate<ItemEstoque>> GetPagedAsync(int index, int size, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .ToPaginateAsync(index, size, 0, cancellationToken);
    }

    public async Task AddAsync(ItemEstoque item, CancellationToken cancellationToken)
    {
        await InsertAsync(item, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ItemEstoque item, CancellationToken cancellationToken)
    {
        Update(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ItemEstoque item, CancellationToken cancellationToken)
    {
        Delete(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
