using Application.UseCases.Veiculos.Dto;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.Veiculos;

public class CreateVeiculoUseCase(
    IVeiculoRepository veiculoRepository)
{
    private readonly IVeiculoRepository _veiculoRepository = veiculoRepository;

    public async Task<VeiculoOutput> ExecuteAsync(CreateVeiculoInput input, CancellationToken cancellationToken)
    {
        var veiculo = new Veiculo(input.ClienteId, input.Placa, input.Modelo, input.Marca, input.Ano);

        await _veiculoRepository.InsertAsync(veiculo, cancellationToken);

        return VeiculoOutput.FromEntity(veiculo);
    }
}