using Domain.Interfaces.Repositories;

namespace Application.UseCases.Clientes;

public class DeleteClienteUseCase(IClienteRepository clienteRepository)
{
    private readonly IClienteRepository _clienteRepository = clienteRepository;

    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException($"Cliente com o ID '{id}' não foi encontrado.");
        
        _clienteRepository.Delete(cliente);

        await _clienteRepository.SaveChangesAsync(cancellationToken);
    }
}