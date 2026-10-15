using Application.Dto;
using Application.UseCases.Clientes.Dto;
using Application.UseCases.Clientes.Validators;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.Clientes;

public class UpdateClienteUseCase(IClienteRepository clienteRepository)
{
    private readonly IClienteRepository _clienteRepository = clienteRepository;

    public async Task<ResultGeneric<ClienteOutput>> ExecuteAsync(Guid id, UpdateClienteInput input, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Cliente com o ID '{id}' não foi encontrado.");

        var errors = new List<string>();
        ClienteValidator.ValidateUpdate(input, errors);

        if (errors.Count > 0)
            return ResultGeneric<ClienteOutput>.Failure(errors);

        var endereco = input.Endereco!.ToValueObject();

        cliente.Update(input.Nome, input.Email, input.Telefone, endereco);

        _clienteRepository.Update(cliente);
        await _clienteRepository.SaveChangesAsync(cancellationToken);

        return ResultGeneric<ClienteOutput>.Success(ClienteOutput.FromEntity(cliente));
    }
}