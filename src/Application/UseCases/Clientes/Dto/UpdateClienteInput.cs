using System.ComponentModel.DataAnnotations;

namespace Application.UseCases.Clientes.Dto;

public record UpdateClienteInput(
    string CPFCNPJ);
