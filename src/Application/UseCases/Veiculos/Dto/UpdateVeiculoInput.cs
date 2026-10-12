using System.ComponentModel.DataAnnotations;

namespace Application.UseCases.Veiculos.Dto;

public record UpdateVeiculoInput(
    string Placa);