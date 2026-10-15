using Domain.Common.Paginate;
using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IItemEstoqueRepository : IBaseRepository<ItemEstoque>
{
    Task<ItemEstoque?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByCodigoAsync(string codigo, CancellationToken cancellationToken);
    Task<IPaginate<ItemEstoque>> GetPagedAsync(int index, int size, CancellationToken cancellationToken);
    Task AddAsync(ItemEstoque item, CancellationToken cancellationToken);
    Task UpdateAsync(ItemEstoque item, CancellationToken cancellationToken);
    Task DeleteAsync(ItemEstoque item, CancellationToken cancellationToken);
}
