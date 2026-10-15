using Application.UseCases.Clientes.Dto;
using Domain.ValueObjects;

namespace Application.UseCases.Clientes.Validators;

public static class ClienteValidator
{
    public static void ValidateCreate(CreateClienteInput input, List<string> errors)
    {
        ValidateNome(input.Nome, errors);
        ValidateDocumento(input.CPFCNPJ, errors);
        EnderecoValidator.Validate(input.Endereco, errors);
    }

    public static void ValidateUpdate(UpdateClienteInput input, List<string> errors)
    {
        ValidateNome(input.Nome, errors);
        EnderecoValidator.Validate(input.Endereco, errors);
    }

    public static void ValidateNome(string? nome, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(nome))
            errors.Add("O nome do cliente é obrigatório.");
    }

    public static void ValidateDocumento(string? cpfCnpj, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(cpfCnpj))
            errors.Add("O documento (CPF/CNPJ) é obrigatório.");
        else if (!Documento.EhValido(cpfCnpj))
            errors.Add("O documento informado é inválido. Informe um CPF ou CNPJ válido.");
    }
}

