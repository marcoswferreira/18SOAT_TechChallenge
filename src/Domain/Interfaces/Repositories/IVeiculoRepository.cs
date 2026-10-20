using Domain.Common.Paginate;
using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IVeiculoRepository : IBaseRepository<Veiculo>
{
    Task<Veiculo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Veiculo?> GetByPlacaAsync(string placa, CancellationToken cancellationToken = default);
    Task<bool> ExistsByPlacaAsync(string placa, CancellationToken cancellationToken = default);
    Task<IEnumerable<Veiculo>> GetByClienteIdAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task<IPaginate<Veiculo>> GetPagedAsync(int index, int size, CancellationToken cancellationToken = default);
}