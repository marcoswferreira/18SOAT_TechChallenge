using Application.UseCases.Clientes.Dto;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.Clientes;

public class GetClienteByIdUseCase(IClienteRepository clienteRepository)
{
    private readonly IClienteRepository _clienteRepository = clienteRepository;

    public async Task<ClienteOutput> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Cliente com o ID '{id}' não foi encontrado.");

        return ClienteOutput.FromEntity(cliente);
    }
}
