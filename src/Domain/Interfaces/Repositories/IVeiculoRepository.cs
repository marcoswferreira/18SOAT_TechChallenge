using Domain.Common.Paginate;
using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IVeiculoRepository : IBaseRepository<Veiculo>
{
    Task<Veiculo?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> ExistsByPlacaAsync(string placa, CancellationToken cancellationToken);
}