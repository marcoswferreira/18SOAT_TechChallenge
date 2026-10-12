using Domain.Interfaces.Repositories;

namespace Application.UseCases.ItensEstoque;

public class DeleteItemEstoqueUseCase(IItemEstoqueRepository itemEstoqueRepository)
{
    private readonly IItemEstoqueRepository _itemEstoqueRepository = itemEstoqueRepository;

    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var item = await _itemEstoqueRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Item de estoque com o ID '{id}' não foi encontrado.");

        if (item.PossuiReserva())
        {
            throw new InvalidOperationException(
                $"O item '{item.Nome}' possui {item.QuantidadeReservada} reservados para ordens de serviço e não pode ser excluído.");
        }

        await _itemEstoqueRepository.DeleteAsync(item, cancellationToken);
    }
}
