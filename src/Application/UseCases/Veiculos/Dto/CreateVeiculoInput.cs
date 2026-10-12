using System.ComponentModel.DataAnnotations;

namespace Application.UseCases.Veiculos.Dto;

public record CreateVeiculoInput(
    Guid ClienteId,
    Placa Placa,
    string Marca,
    string Modelo,
    int Ano);