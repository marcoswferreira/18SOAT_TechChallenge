namespace Application.UseCases.Veiculos.Dto;

public record UpdateVeiculoInput(
    string Placa,
    string Marca,
    string Modelo,
    int Ano
);