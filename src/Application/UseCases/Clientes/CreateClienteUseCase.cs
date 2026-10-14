using Application.Dto;
using Application.UseCases.Clientes.Dto;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;

namespace Application.UseCases.Clientes;

public class CreateClienteUseCase(IClienteRepository clienteRepository)
{
    private readonly IClienteRepository _clienteRepository = clienteRepository;

    public async Task<ResultGeneric<ClienteOutput>> ExecuteAsync(CreateClienteInput input, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(input.Nome))
            errors.Add("O nome do cliente é obrigatório.");

        if (string.IsNullOrWhiteSpace(input.CPFCNPJ))
            errors.Add("O documento (CPF/CNPJ) é obrigatório.");
        else if (!Documento.EhValido(input.CPFCNPJ))
            errors.Add("O documento informado é inválido. Informe um CPF ou CNPJ válido.");

        if (input.Endereco is null)
        {
            errors.Add("O endereço é obrigatório.");
        }
        else
        {
            if (string.IsNullOrWhiteSpace(input.Endereco.Logradouro))
                errors.Add("O logradouro do endereço é obrigatório.");

            if (string.IsNullOrWhiteSpace(input.Endereco.Numero))
                errors.Add("O número do endereço é obrigatório.");

            if (string.IsNullOrWhiteSpace(input.Endereco.Bairro))
                errors.Add("O bairro do endereço é obrigatório.");

            if (string.IsNullOrWhiteSpace(input.Endereco.Cidade))
                errors.Add("A cidade do endereço é obrigatória.");

            if (string.IsNullOrWhiteSpace(input.Endereco.Estado))
                errors.Add("O estado (UF) do endereço é obrigatório.");
            else if (!Endereco.EhEstadoValido(input.Endereco.Estado))
                errors.Add($"O estado (UF) '{input.Endereco.Estado}' é inválido.");

            if (string.IsNullOrWhiteSpace(input.Endereco.Cep))
                errors.Add("O CEP do endereço é obrigatório.");
            else if (!Endereco.EhCepValido(input.Endereco.Cep))
                errors.Add("O CEP deve conter exatamente 8 dígitos numéricos.");
        }

        if (errors.Count > 0)
            return ResultGeneric<ClienteOutput>.Failure(errors);

        var documento = new Documento(input.CPFCNPJ);
        var endereco = input.Endereco!.ToValueObject();

        var clienteExists = await _clienteRepository.ExistsByDocumentoAsync(documento.Valor, cancellationToken);
        if (clienteExists)
        {
            return ResultGeneric<ClienteOutput>.Failure($"Já existe um cliente cadastrado com o documento '{input.CPFCNPJ}'.");
        }

        var cliente = new Cliente(input.Nome, documento, input.Email, input.Telefone, endereco);

        await _clienteRepository.InsertAsync(cliente, cancellationToken);
        await _clienteRepository.SaveChangesAsync(cancellationToken);

        return ResultGeneric<ClienteOutput>.Success(ClienteOutput.FromEntity(cliente));
    }
}