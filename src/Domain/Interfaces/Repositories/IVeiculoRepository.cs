using Domain.Common.Paginate;
using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IVeiculoRepository : IBaseRepository<Veiculo>
{
    Task<Veiculo?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Veiculo?> GetByPlacaAsync(string placa, CancellationToken cancellationToken);
    Task<Veiculo?> GetByRefreshTokenAsync(string refreshTokenHash, CancellationToken cancellationToken);
    Task AddAsync(Veiculo veiculo, CancellationToken cancellationToken);
    Task UpdateAsync(Veiculo veiculo, CancellationToken cancellationToken);
    Task DeleteAsync(Veiculo veiculo, CancellationToken cancellationToken);
    Task<IPaginate<Veiculo>> GetPagedAsync(int index, int size, CancellationToken cancellationToken);
}