namespace Domain.ValueObjects;

public record ItemServico(string Descricao, decimal ValorUnitario, int Quantidade) : IValueObject
{
    public decimal ValorTotal => ValorUnitario * Quantidade;

    public static ItemServico Criar(string descricao, decimal valorUnitario, int quantidade)
    {
        if (valorUnitario < 0)
        {
            throw new ArgumentException("O valor unitário do serviço não pode ser negativo.");
        }

        if (quantidade <= 0)
        {
            throw new ArgumentException("A quantidade deve ser maior que zero.");
        }

        return new ItemServico(descricao, valorUnitario, quantidade);
    }
}
