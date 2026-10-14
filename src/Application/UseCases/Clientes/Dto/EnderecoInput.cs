using Domain.ValueObjects;

namespace Application.UseCases.Clientes.Dto;

public record EnderecoInput(
    string Logradouro,
    string Numero,
    string? Complemento,
    string Bairro,
    string Cidade,
    string Estado,
    string Cep
)
{
    public Endereco ToValueObject() => new(
        Logradouro,
        Numero,
        Complemento,
        Bairro,
        Cidade,
        Estado,
        Cep
    );
}
