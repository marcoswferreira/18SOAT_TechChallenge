namespace Domain.ValueObjects;

public record ItemPeca(string NomePeca, decimal ValorUnitario, int Quantidade) : IValueObject
{
    public decimal ValorTotal => ValorUnitario * Quantidade;

    public static ItemPeca Criar(string nomePeca, decimal valorUnitario, int quantidade)
    {
        if (valorUnitario < 0)
            throw new ArgumentException("O valor unitário da peça não pode ser negativo.");

        if (quantidade <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.");

        return new ItemPeca(nomePeca, valorUnitario, quantidade);
    }
}
