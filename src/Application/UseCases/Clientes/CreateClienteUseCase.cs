using Application.Dto;
using Application.UseCases.Clientes.Dto;
using Application.UseCases.Clientes.Validators;
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
        ClienteValidator.ValidateCreate(input, errors);

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