using Domain.Entities;
using Domain.Enums;

namespace Domain.ValueObjects;

public record ItemPeca : IValueObject
{
    public Guid ItemEstoqueId { get; init; }
    public string NomePeca { get; init; } = string.Empty;
    public decimal ValorUnitario { get; init; }
    public decimal Quantidade { get; init; }

    public decimal ValorTotal => ValorUnitario * Quantidade;

    // Construtor protegido para EF Core / desserialização
    protected ItemPeca() { }

    public ItemPeca(Guid itemEstoqueId, string nomePeca, decimal valorUnitario, decimal quantidade)
    {
        if (itemEstoqueId == Guid.Empty)
            throw new ArgumentException("O ID do item de estoque é obrigatório.", nameof(itemEstoqueId));

        if (string.IsNullOrWhiteSpace(nomePeca))
            throw new ArgumentException("O nome da peça é obrigatório.", nameof(nomePeca));

        if (valorUnitario < 0)
            throw new ArgumentException("O valor unitário da peça não pode ser negativo.", nameof(valorUnitario));

        if (quantidade <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(quantidade));

        ItemEstoqueId = itemEstoqueId;
        NomePeca = nomePeca.Trim();
        ValorUnitario = valorUnitario;
        Quantidade = quantidade;
    }

    public ItemPeca(string nomePeca, decimal valorUnitario, decimal quantidade)
        : this(Guid.NewGuid(), nomePeca, valorUnitario, quantidade)
    {
    }

    /// <summary>
    /// Cria o ItemPeca a partir de uma entidade ItemEstoque, aplicando as validações de unidade de medida.
    /// </summary>
    public static ItemPeca Criar(ItemEstoque itemEstoque, decimal quantidade, decimal? valorUnitario = null)
    {
        ArgumentNullException.ThrowIfNull(itemEstoque);

        if (quantidade <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(quantidade));

        if (itemEstoque.UnidadeMedida == UnidadeMedida.Unidade && quantidade % 1 != 0)
            throw new ArgumentException($"O item '{itemEstoque.Nome}' é controlado por unidade, informe uma quantidade inteira.", nameof(quantidade));

        var preco = valorUnitario ?? itemEstoque.PrecoUnitario;

        return new ItemPeca(itemEstoque.Id, itemEstoque.Nome, preco, quantidade);
    }

    public static ItemPeca Criar(Guid itemEstoqueId, string nomePeca, decimal valorUnitario, decimal quantidade)
    {
        return new ItemPeca(itemEstoqueId, nomePeca, valorUnitario, quantidade);
    }

    public static ItemPeca Criar(string nomePeca, decimal valorUnitario, decimal quantidade)
    {
        return new ItemPeca(Guid.NewGuid(), nomePeca, valorUnitario, quantidade);
    }
}
