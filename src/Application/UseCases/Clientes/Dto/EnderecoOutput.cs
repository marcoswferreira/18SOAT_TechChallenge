using Domain.ValueObjects;

namespace Application.UseCases.Clientes.Dto;

public record EnderecoOutput(
    string Logradouro,
    string Numero,
    string? Complemento,
    string Bairro,
    string Cidade,
    string Estado,
    string Cep,
    string Formatado
)
{
    public static EnderecoOutput FromValueObject(Endereco endereco) => new(
        endereco.Logradouro,
        endereco.Numero,
        endereco.Complemento,
        endereco.Bairro,
        endereco.Cidade,
        endereco.Estado,
        endereco.Cep,
        endereco.Formatado
    );
}
