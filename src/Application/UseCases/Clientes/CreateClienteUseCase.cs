using Application.UseCases.Clientes.Dto;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.Clientes;

public class CreateClienteUseCase(
    IClienteRepository clienteRepository)
{
    private readonly IClienteRepository _clienteRepository = clienteRepository;

    public async Task<ClienteOutput> ExecuteAsync(CreateClienteInput input, CancellationToken cancellationToken)
    {
        //Regras


        var cliente = new Cliente(input.Nome, input.CPFCNPJ, input.Email, input.Telefone, input.Endereco);

        await _clienteRepository.InsertAsync(cliente, cancellationToken);

        return ClienteOutput.FromEntity(cliente);
    }
}