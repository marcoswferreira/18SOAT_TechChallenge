using Application.UseCases.Clientes.Dto;
using Domain.Common.Paginate;
using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.Clientes;

public class GetClientesPagedUseCase(IClienteRepository clienteRepository)
{
    private readonly IClienteRepository _clienteRepository = clienteRepository;

    public async Task<IPaginate<ClienteOutput>> ExecuteAsync(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        var pagedClientes = await _clienteRepository.GetPagedAsync(pageIndex, pageSize, cancellationToken);

        return new Paginate<Cliente, ClienteOutput>(
            pagedClientes,
            clientes => clientes.Select(ClienteOutput.FromEntity));
    }
}

