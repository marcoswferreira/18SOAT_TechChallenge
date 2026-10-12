using Application.UseCases.ItensEstoque.Dto;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.ItensEstoque;

public class UpdateItemEstoqueUseCase(IItemEstoqueRepository itemEstoqueRepository)
{
    private readonly IItemEstoqueRepository _itemEstoqueRepository = itemEstoqueRepository;

    public async Task<ItemEstoqueOutput> ExecuteAsync(Guid id, UpdateItemEstoqueInput input, CancellationToken cancellationToken)
    {
        var item = await _itemEstoqueRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Item de estoque com o ID '{id}' não foi encontrado.");

        // quantidade não muda por aqui, só pela entrada de estoque
        item.Atualizar(input.Nome, input.Descricao, input.UnidadeMedida, input.PrecoUnitario);

        await _itemEstoqueRepository.UpdateAsync(item, cancellationToken);

        return ItemEstoqueOutput.FromEntity(item);
    }
}
