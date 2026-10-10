using Domain.Entities;

namespace Application.UseCases.Clientes.Dto;

public record ClienteOutput(
    Guid Id,
    string Nome,
    string CPFCNPJ,
    string? Email,
    string? Telefone,
    string Endereco
)
{
    public static ClienteOutput FromEntity(Cliente cliente) => new(
        cliente.Id,
        cliente.Nome,
        cliente.CPFCNPJ,
        cliente.Email,
        cliente.Telefone,
        cliente.Endereco
    );
}