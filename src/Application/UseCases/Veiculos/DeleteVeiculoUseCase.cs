using Domain.Interfaces.Repositories;

namespace Application.UseCases.Veiculos;

public class DeleteVeiculoUseCase(IVeiculoRepository veiculoRepository)
{
    private readonly IVeiculoRepository _veiculoRepository = veiculoRepository;

    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var veiculo = await _veiculoRepository.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException($"Veículo com o ID '{id}' não foi encontrado.");
        
        _veiculoRepository.Delete(veiculo);

        await _veiculoRepository.SaveChangesAsync(cancellationToken);
    }
}