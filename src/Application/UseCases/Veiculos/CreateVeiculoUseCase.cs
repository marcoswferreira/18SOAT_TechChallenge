using Application.Dto;
using Application.UseCases.Veiculos.Dto;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;

namespace Application.UseCases.Veiculos;

public class CreateVeiculoUseCase(
    IVeiculoRepository veiculoRepository,
    IClienteRepository clienteRepository)
{
    private readonly IVeiculoRepository _veiculoRepository = veiculoRepository;
    private readonly IClienteRepository _clienteRepository = clienteRepository;

    public async Task<ResultGeneric<VeiculoOutput>> ExecuteAsync(CreateVeiculoInput input, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        if (input.ClienteId == Guid.Empty)
            errors.Add("O identificador do cliente é obrigatório.");

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

        var cliente = await _clienteRepository.GetByIdAsync(input.ClienteId, cancellationToken);
        if (cliente is null)
            errors.Add($"Cliente com o ID '{input.ClienteId}' não foi encontrado.");

        var placa = new Placa(input.Placa);

        var veiculoExists = await _veiculoRepository.ExistsByPlacaAsync(placa.Valor, cancellationToken);
        if (veiculoExists)
            errors.Add($"Já existe um veículo cadastrado com a placa '{placa.Valor}'.");

        if (errors.Count > 0)
            return ResultGeneric<VeiculoOutput>.Failure(errors);

        var veiculo = new Veiculo(input.ClienteId, placa, input.Marca, input.Modelo, input.Ano);

        await _veiculoRepository.InsertAsync(veiculo, cancellationToken);
        await _veiculoRepository.SaveChangesAsync(cancellationToken);

        return ResultGeneric<VeiculoOutput>.Success(VeiculoOutput.FromEntity(veiculo));
    }
}