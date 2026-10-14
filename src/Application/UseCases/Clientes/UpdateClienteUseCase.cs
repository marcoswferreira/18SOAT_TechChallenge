using Application.Dto;
using Application.UseCases.Clientes.Dto;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;

namespace Application.UseCases.Clientes;

public class UpdateClienteUseCase(IClienteRepository clienteRepository)
{
    private readonly IClienteRepository _clienteRepository = clienteRepository;

    public async Task<ResultGeneric<ClienteOutput>> ExecuteAsync(Guid id, UpdateClienteInput input, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Cliente com o ID '{id}' não foi encontrado.");

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(input.Nome))
            errors.Add("O nome do cliente é obrigatório.");

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

        var endereco = input.Endereco!.ToValueObject();

        cliente.Update(input.Nome, input.Email, input.Telefone, endereco);

        _clienteRepository.Update(cliente);
        await _clienteRepository.SaveChangesAsync(cancellationToken);

        return ResultGeneric<ClienteOutput>.Success(ClienteOutput.FromEntity(cliente));
    }
}