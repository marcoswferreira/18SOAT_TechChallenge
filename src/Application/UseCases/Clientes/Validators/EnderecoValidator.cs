using Application.UseCases.Clientes.Dto;
using Domain.ValueObjects;

namespace Application.UseCases.Clientes.Validators;

public static class EnderecoValidator
{
    public static void Validate(EnderecoInput? endereco, List<string> errors)
    {
        if (endereco is null)
        {
            errors.Add("O endereço é obrigatório.");
            return;
        }

        if (string.IsNullOrWhiteSpace(endereco.Logradouro))
            errors.Add("O logradouro do endereço é obrigatório.");

        if (string.IsNullOrWhiteSpace(endereco.Numero))
            errors.Add("O número do endereço é obrigatório.");

        if (string.IsNullOrWhiteSpace(endereco.Bairro))
            errors.Add("O bairro do endereço é obrigatório.");

        if (string.IsNullOrWhiteSpace(endereco.Cidade))
            errors.Add("A cidade do endereço é obrigatória.");

        if (string.IsNullOrWhiteSpace(endereco.Estado))
            errors.Add("O estado (UF) do endereço é obrigatório.");
        else if (!Endereco.EhEstadoValido(endereco.Estado))
            errors.Add($"O estado (UF) '{endereco.Estado}' é inválido.");

        if (string.IsNullOrWhiteSpace(endereco.Cep))
            errors.Add("O CEP do endereço é obrigatório.");
        else if (!Endereco.EhCepValido(endereco.Cep))
            errors.Add("O CEP deve conter exatamente 8 dígitos numéricos.");
    }
}

