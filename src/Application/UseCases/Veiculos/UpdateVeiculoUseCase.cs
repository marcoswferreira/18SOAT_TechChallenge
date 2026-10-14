using Application.Dto;
using Application.UseCases.Veiculos.Dto;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;

namespace Application.UseCases.Veiculos;

public class UpdateVeiculoUseCase(IVeiculoRepository veiculoRepository)
{
    private readonly IVeiculoRepository _veiculoRepository = veiculoRepository;

    public async Task<ResultGeneric<VeiculoOutput>> ExecuteAsync(Guid id, UpdateVeiculoInput input, CancellationToken cancellationToken)
    {
        var veiculo = await _veiculoRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Veículo com o ID '{id}' não foi encontrado.");

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(input.Placa))
            errors.Add("A placa do veículo é obrigatória.");
        else if (!Placa.EhValida(input.Placa))
            errors.Add("A placa informada é inválida.");

        if (string.IsNullOrWhiteSpace(input.Marca))
            errors.Add("A marca do veículo é obrigatória.");

        if (string.IsNullOrWhiteSpace(input.Modelo))
            errors.Add("O modelo do veículo é obrigatório.");

        var limiteAnoMaximo = DateTime.UtcNow.Year + 1;
        if (input.Ano < 1900 || input.Ano > limiteAnoMaximo)
            errors.Add($"O ano do veículo deve ser entre 1900 e {limiteAnoMaximo}.");

        if (errors.Count > 0)
            return ResultGeneric<VeiculoOutput>.Failure(errors);

        var novaPlaca = new Placa(input.Placa);

        if (veiculo.Placa != novaPlaca && await _veiculoRepository.ExistsByPlacaAsync(novaPlaca.Valor, cancellationToken))
        {
            return ResultGeneric<VeiculoOutput>.Failure($"Já existe um veículo cadastrado com a placa '{novaPlaca.Valor}'.");
        }

        veiculo.Update(novaPlaca, input.Marca, input.Modelo, input.Ano);

        _veiculoRepository.Update(veiculo);
        await _veiculoRepository.SaveChangesAsync(cancellationToken);

        return ResultGeneric<VeiculoOutput>.Success(VeiculoOutput.FromEntity(veiculo));
    }
}