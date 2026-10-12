using Application.UseCases.Veiculos.Dto;
using Domain.Constants;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.Veiculos;

public class UpdateVeiculoUseCase(IVeiculoRepository veiculoRepository)
{
    private readonly IVeiculoRepository _veiculoRepository = veiculoRepository;

    public async Task<VeiculoOutput> ExecuteAsync(Guid id, UpdateVeiculoInput input, CancellationToken cancellationToken)
    {
        var veiculo = await _veiculoRepository.GetByIdAsync(id, cancellationToken);
        if (veiculo is null)
        {
            throw new KeyNotFoundException($"Veículo com o ID '{id}' não foi encontrado.");
        }

        var veiculoExists = await _veiculoRepository.ExistsByPlacaAsync(veiculo.Placa.Valor, cancellationToken);
        if (veiculoExists)
        {
            throw new InvalidOperationException($"Já existe um veículo cadastrado com a placa '{veiculo.Placa.Valor}'.");
        }

        _veiculoRepository.Update(veiculo);

        return VeiculoOutput.FromEntity(veiculo);
    }
}