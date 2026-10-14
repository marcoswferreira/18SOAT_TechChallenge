using Domain.Common.Paginate;
using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IVeiculoRepository : IBaseRepository<Veiculo>
{
    Task<Veiculo?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Veiculo?> GetByPlacaAsync(string placa, CancellationToken cancellationToken);
    Task<bool> ExistsByPlacaAsync(string placa, CancellationToken cancellationToken);
    Task<IEnumerable<Veiculo>> GetByClienteIdAsync(Guid clienteId, CancellationToken cancellationToken);
    Task<IPaginate<Veiculo>> GetPagedAsync(int index, int size, CancellationToken cancellationToken);
}