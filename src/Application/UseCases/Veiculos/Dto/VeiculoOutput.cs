using Domain.Entities;

namespace Application.UseCases.Veiculos.Dto;

public record VeiculoOutput(
    Guid Id,
    Guid ClienteId,
    string Placa,
    string Marca,
    string Modelo,
    int Ano)
{
    public static VeiculoOutput FromEntity(Veiculo veiculo) => new(
        veiculo.Id,
        veiculo.ClienteId,
        veiculo.Placa.Valor,
        veiculo.Marca,
        veiculo.Modelo,
        veiculo.Ano
    );
}