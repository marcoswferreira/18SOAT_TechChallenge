using Application.UseCases.Clientes.Dto;
using Domain.Constants;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.Clientes;

public class UpdateClienteUseCase(IClienteRepository clienteRepository)
{
    private readonly IClienteRepository _clienteRepository = clienteRepository;

    public async Task<ClienteOutput> ExecuteAsync(Guid id, UpdateClienteInput input, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken);
        if (cliente is null)
        {
            throw new KeyNotFoundException($"Cliente com o ID '{id}' não foi encontrado.");
        }

        var clienteExists = await _clienteRepository.ExistsByCPFCNPJAsync(cliente.CPFCNPJ, cancellationToken);
        if (clienteExists)
        {
            throw new InvalidOperationException($"Já existe um cliente cadastrado com o CPFCNPJ '{cliente.CPFCNPJ}'.");
        }

        _clienteRepository.Update(cliente);

        return ClienteOutput.FromEntity(cliente);
    }
}