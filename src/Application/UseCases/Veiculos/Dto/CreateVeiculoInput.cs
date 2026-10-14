namespace Application.UseCases.Veiculos.Dto;

public record CreateVeiculoInput(
    Guid ClienteId,
    string Placa,
    string Marca,
    string Modelo,
    int Ano
);