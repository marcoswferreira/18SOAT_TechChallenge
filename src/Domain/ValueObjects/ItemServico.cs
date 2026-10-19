using Domain.Entities;

namespace Domain.ValueObjects;

public record ItemServico : IValueObject
{
    public Guid? ServicoId { get; init; }
    public string Descricao { get; init; } = string.Empty;
    public decimal ValorUnitario { get; init; }
    public int Quantidade { get; init; }

    public decimal ValorTotal => ValorUnitario * Quantidade;

    // Construtor protegido para EF Core / desserialização
    protected ItemServico() { }

    public ItemServico(Guid? servicoId, string descricao, decimal valorUnitario, int quantidade)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("A descrição do serviço é obrigatória.", nameof(descricao));

        if (valorUnitario < 0)
            throw new ArgumentException("O valor unitário do serviço não pode ser negativo.", nameof(valorUnitario));

        if (quantidade <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(quantidade));

        ServicoId = servicoId;
        Descricao = descricao.Trim();
        ValorUnitario = valorUnitario;
        Quantidade = quantidade;
    }

    public ItemServico(string descricao, decimal valorUnitario, int quantidade)
        : this(null, descricao, valorUnitario, quantidade)
    {
    }

    /// <summary>
    /// Cria o ItemServico a partir de uma entidade de catálogo Servico.
    /// </summary>
    public static ItemServico Criar(Servico servico, decimal valorUnitario, int quantidade = 1)
    {
        ArgumentNullException.ThrowIfNull(servico);
        return new ItemServico(servico.Id, servico.Nome, valorUnitario, quantidade);
    }

    public static ItemServico Criar(string descricao, decimal valorUnitario, int quantidade)
    {
        return new ItemServico(null, descricao, valorUnitario, quantidade);
    }
}
