using Domain.Common.Entities;
using Domain.Enums;

namespace Domain.Entities;

public class ItemEstoque : SoftDeleteBaseEntity, IAggregateRoot
{
    public string Codigo { get; private set; } = string.Empty;
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public UnidadeMedida UnidadeMedida { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public decimal QuantidadeEmEstoque { get; private set; }
    public decimal QuantidadeReservada { get; private set; }

    // mapeado pro xmin do postgres, evita duas OS reservarem o mesmo saldo ao mesmo tempo
    public uint Versao { get; private set; }

    public decimal QuantidadeDisponivel => QuantidadeEmEstoque - QuantidadeReservada;

    protected ItemEstoque() { }

    public ItemEstoque(string codigo, string nome, string? descricao, UnidadeMedida unidadeMedida, decimal precoUnitario)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(codigo, nameof(codigo));

        Codigo = codigo.Trim().ToUpperInvariant();
        Atualizar(nome, descricao, unidadeMedida, precoUnitario);
    }

    public void Atualizar(string nome, string? descricao, UnidadeMedida unidadeMedida, decimal precoUnitario)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome, nameof(nome));

        if (!Enum.IsDefined(unidadeMedida))
            throw new ArgumentException("Unidade de medida inválida.", nameof(unidadeMedida));

        if (precoUnitario < 0)
            throw new ArgumentException("O preço unitário não pode ser negativo.", nameof(precoUnitario));

        // se já tem saldo fracionado (ex.: 2,5 L) não dá pra virar unidade
        if (unidadeMedida == UnidadeMedida.Unidade && (!EhInteiro(QuantidadeEmEstoque) || !EhInteiro(QuantidadeReservada)))
            throw new InvalidOperationException(
                $"O item '{nome}' possui saldo fracionado e não pode ser alterado para {UnidadeMedida.Unidade}.");

        Nome = nome.Trim();
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        UnidadeMedida = unidadeMedida;
        PrecoUnitario = precoUnitario;
    }

    public void AdicionarEstoque(decimal quantidade)
    {
        ValidarQuantidade(quantidade);
        QuantidadeEmEstoque += quantidade;
    }

    public void Reservar(decimal quantidade)
    {
        ValidarQuantidade(quantidade);

        if (quantidade > QuantidadeDisponivel)
            throw new InvalidOperationException(
                $"Estoque insuficiente para '{Nome}'. Disponível: {QuantidadeDisponivel}, solicitado: {quantidade}.");

        QuantidadeReservada += quantidade;
    }

    public void LiberarReserva(decimal quantidade)
    {
        ValidarQuantidade(quantidade);

        if (quantidade > QuantidadeReservada)
            throw new InvalidOperationException(
                $"Não é possível liberar {quantidade} de '{Nome}', só existem {QuantidadeReservada} reservados.");

        QuantidadeReservada -= quantidade;
    }

    // a baixa sai do que já estava reservado pra OS, por isso diminui as duas quantidades
    public void BaixarReserva(decimal quantidade)
    {
        ValidarQuantidade(quantidade);

        if (quantidade > QuantidadeReservada)
            throw new InvalidOperationException(
                $"Não é possível dar baixa em {quantidade} de '{Nome}', só existem {QuantidadeReservada} reservados.");

        QuantidadeReservada -= quantidade;
        QuantidadeEmEstoque -= quantidade;
    }

    public bool PossuiReserva() => QuantidadeReservada > 0;

    private void ValidarQuantidade(decimal quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(quantidade));

        // litro e quilo podem ser fracionados, unidade não
        if (UnidadeMedida == UnidadeMedida.Unidade && !EhInteiro(quantidade))
            throw new ArgumentException($"O item '{Nome}' é controlado por unidade, informe uma quantidade inteira.", nameof(quantidade));
    }

    private static bool EhInteiro(decimal valor) => valor % 1 == 0;
}
