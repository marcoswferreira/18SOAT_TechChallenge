using Application.UseCases.Veiculos.Dto;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.Veiculos;

public class GetVeiculoByIdUseCase(
    IVeiculoRepository veiculoRepository)
{
    private readonly IVeiculoRepository _veiculoRepository = veiculoRepository;

    public async Task<VeiculoOutput> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        //Usar Query ???

        var veiculo = await _veiculoRepository.GetByIdAsync(id, cancellationToken);

        return veiculo is null ? throw new KeyNotFoundException($"Veículo com o ID '{id}' não foi encontrado.") : VeiculoOutput.FromEntity(veiculo);
    }
}
