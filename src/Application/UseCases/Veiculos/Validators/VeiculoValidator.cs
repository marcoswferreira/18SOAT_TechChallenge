using Application.UseCases.Veiculos.Dto;
using Domain.ValueObjects;

namespace Application.UseCases.Veiculos.Validators;

public static class VeiculoValidator
{
    public static void ValidateCreate(CreateVeiculoInput input, List<string> errors)
    {
        if (input.ClienteId == Guid.Empty)
            errors.Add("O identificador do cliente é obrigatório.");

        ValidateDadosVeiculo(input.Placa, input.Marca, input.Modelo, input.Ano, errors);
    }

    public static void ValidateUpdate(UpdateVeiculoInput input, List<string> errors)
    {
        ValidateDadosVeiculo(input.Placa, input.Marca, input.Modelo, input.Ano, errors);
    }

    public static void ValidateDadosVeiculo(string? placa, string? marca, string? modelo, int ano, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(placa))
            errors.Add("A placa do veículo é obrigatória.");
        else if (!Placa.EhValida(placa))
            errors.Add("A placa informada é inválida.");

        if (string.IsNullOrWhiteSpace(marca))
            errors.Add("A marca do veículo é obrigatória.");

        if (string.IsNullOrWhiteSpace(modelo))
            errors.Add("O modelo do veículo é obrigatório.");

        var limiteAnoMaximo = DateTime.UtcNow.Year + 1;
        if (ano < 1900 || ano > limiteAnoMaximo)
            errors.Add($"O ano do veículo deve ser entre 1900 e {limiteAnoMaximo}.");
    }
}

