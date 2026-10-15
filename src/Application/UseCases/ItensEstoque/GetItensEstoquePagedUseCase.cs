using Application.UseCases.ItensEstoque.Dto;
using Domain.Common.Paginate;
using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.ItensEstoque;

public class GetItensEstoquePagedUseCase(IItemEstoqueRepository itemEstoqueRepository)
{
    private readonly IItemEstoqueRepository _itemEstoqueRepository = itemEstoqueRepository;

    public async Task<IPaginate<ItemEstoqueOutput>> ExecuteAsync(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        var pagedItens = await _itemEstoqueRepository.GetPagedAsync(pageIndex, pageSize, cancellationToken);

        return new Paginate<ItemEstoque, ItemEstoqueOutput>(
            pagedItens,
            itens => itens.Select(ItemEstoqueOutput.FromEntity));
    }
}
