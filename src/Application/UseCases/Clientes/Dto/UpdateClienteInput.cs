namespace Application.UseCases.Clientes.Dto;

public record UpdateClienteInput(
    string Nome,
    string? Email,
    string? Telefone,
    EnderecoInput Endereco
);
