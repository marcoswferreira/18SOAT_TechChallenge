using Application.UseCases.Clientes.Dto;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.Clientes;

public class GetClienteByIdUseCase(
    IClienteRepository clienteRepository)
{
    private readonly IClienteRepository _clienteRepository = clienteRepository;

    public async Task<ClienteOutput> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        //Usar Query ???

        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken);

        return cliente is null ? throw new KeyNotFoundException($"Cliente com o ID '{id}' não foi encontrado.") : ClienteOutput.FromEntity(cliente);
    }
}
