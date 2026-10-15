using Application.UseCases.ItensEstoque.Dto;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.ItensEstoque;

public class RegistrarEntradaEstoqueUseCase(IItemEstoqueRepository itemEstoqueRepository)
{
    private readonly IItemEstoqueRepository _itemEstoqueRepository = itemEstoqueRepository;

    public async Task<ItemEstoqueOutput> ExecuteAsync(Guid id, EntradaEstoqueInput input, CancellationToken cancellationToken)
    {
        var item = await _itemEstoqueRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Item de estoque com o ID '{id}' não foi encontrado.");

        item.AdicionarEstoque(input.Quantidade);

        await _itemEstoqueRepository.UpdateAsync(item, cancellationToken);

        return ItemEstoqueOutput.FromEntity(item);
    }
}
