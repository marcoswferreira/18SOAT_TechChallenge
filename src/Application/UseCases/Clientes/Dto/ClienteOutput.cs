using Domain.Entities;

namespace Application.UseCases.Clientes.Dto;

public record ClienteOutput(
    Guid Id,
    string Nome,
    string CPFCNPJ,
    string? Email,
    string? Telefone,
    EnderecoOutput Endereco,
    DateTime CreatedAt
)
{
    public static ClienteOutput FromEntity(Cliente cliente) => new(
        cliente.Id,
        cliente.Nome,
        cliente.Documento.Mascarado,
        cliente.Email,
        cliente.Telefone,
        EnderecoOutput.FromValueObject(cliente.Endereco),
        cliente.CreatedAt
    );
}