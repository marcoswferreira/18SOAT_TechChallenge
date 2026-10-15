using Application.UseCases.ItensEstoque.Dto;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.ItensEstoque;

public class GetItemEstoqueByIdUseCase(IItemEstoqueRepository itemEstoqueRepository)
{
    private readonly IItemEstoqueRepository _itemEstoqueRepository = itemEstoqueRepository;

    public async Task<ItemEstoqueOutput> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var item = await _itemEstoqueRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Item de estoque com o ID '{id}' não foi encontrado.");

        return ItemEstoqueOutput.FromEntity(item);
    }
}
