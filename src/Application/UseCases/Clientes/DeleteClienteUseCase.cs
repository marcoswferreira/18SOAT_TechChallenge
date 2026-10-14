using Application.Dto;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.Clientes;

public class DeleteClienteUseCase(IClienteRepository clienteRepository)
{
    private readonly IClienteRepository _clienteRepository = clienteRepository;

    public async Task<Result> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.GetByIdWithVeiculosAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Cliente com o ID '{id}' não foi encontrado.");

        if (cliente.Veiculos.Count > 0)
        {
            return Result.Failure($"Não é possível remover o cliente pois ele possui {cliente.Veiculos.Count} veículo(s) vinculado(s).");
        }

        _clienteRepository.Delete(cliente);
        await _clienteRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}