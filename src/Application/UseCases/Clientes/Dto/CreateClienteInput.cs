namespace Application.UseCases.Clientes.Dto;

public record CreateClienteInput(
    string Nome,
    string CPFCNPJ,
    string? Email,
    string? Telefone,
    EnderecoInput Endereco
);