using Application.UseCases.ItensEstoque.Dto;
using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.ItensEstoque;

public class CreateItemEstoqueUseCase(IItemEstoqueRepository itemEstoqueRepository)
{
    private readonly IItemEstoqueRepository _itemEstoqueRepository = itemEstoqueRepository;

    public async Task<ItemEstoqueOutput> ExecuteAsync(CreateItemEstoqueInput input, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input.Codigo) || string.IsNullOrWhiteSpace(input.Nome))
        {
            throw new ArgumentException("Código e nome são obrigatórios.");
        }

        var codigoExiste = await _itemEstoqueRepository.ExistsByCodigoAsync(input.Codigo, cancellationToken);
        if (codigoExiste)
        {
            throw new InvalidOperationException($"Já existe um item cadastrado com o código '{input.Codigo}'.");
        }

        var item = new ItemEstoque(input.Codigo, input.Nome, input.Descricao, input.UnidadeMedida, input.PrecoUnitario);

        await _itemEstoqueRepository.AddAsync(item, cancellationToken);

        return ItemEstoqueOutput.FromEntity(item);
    }
}
