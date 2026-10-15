using Domain.Entities;
using Domain.Enums;

namespace Domain.UnitTests.Entities;

public class ItemEstoqueTests
{
    private static ItemEstoque CriarItem(decimal estoqueInicial = 0, UnidadeMedida unidade = UnidadeMedida.Unidade)
    {
        var item = new ItemEstoque("flt-001", "Filtro de óleo", "Filtro para motor 1.0", unidade, 35.90m);
        if (estoqueInicial > 0)
            item.AdicionarEstoque(estoqueInicial);
        return item;
    }

    // -------------------------------------------------------------------------
    // Constructor / Atualizar
    // -------------------------------------------------------------------------

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateAndNormalize()
    {
        var item = new ItemEstoque("  flt-001 ", "  Filtro de óleo ", "  ", UnidadeMedida.Unidade, 35.90m);

        Assert.NotEqual(Guid.Empty, item.Id);
        Assert.Equal("FLT-001", item.Codigo);
        Assert.Equal("Filtro de óleo", item.Nome);
        Assert.Null(item.Descricao);
        Assert.Equal(UnidadeMedida.Unidade, item.UnidadeMedida);
        Assert.Equal(0, item.QuantidadeEmEstoque);
        Assert.Equal(0, item.QuantidadeReservada);
        Assert.False(item.IsDeleted);
    }

    [Theory]
    [InlineData("", "Nome", 10)]
    [InlineData("COD", "", 10)]
    [InlineData("COD", "Nome", -1)]
    public void Constructor_WithInvalidData_ShouldThrowArgumentException(string codigo, string nome, decimal preco)
    {
        Assert.ThrowsAny<ArgumentException>(() => new ItemEstoque(codigo, nome, null, UnidadeMedida.Unidade, preco));
    }

    [Fact]
    public void Constructor_WithInvalidUnidade_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ItemEstoque("COD", "Nome", null, (UnidadeMedida)99, 10));
    }

    [Fact]
    public void Atualizar_ShouldChangeDataButKeepCodigoAndSaldo()
    {
        var item = CriarItem(estoqueInicial: 10);

        item.Atualizar("Óleo 5W30", "Sintético", UnidadeMedida.Litro, 49.90m);

        Assert.Equal("FLT-001", item.Codigo);
        Assert.Equal("Óleo 5W30", item.Nome);
        Assert.Equal("Sintético", item.Descricao);
        Assert.Equal(UnidadeMedida.Litro, item.UnidadeMedida);
        Assert.Equal(49.90m, item.PrecoUnitario);
        Assert.Equal(10, item.QuantidadeEmEstoque);
    }

    [Fact]
    public void Atualizar_ParaUnidadeComSaldoFracionado_ShouldThrowInvalidOperationException()
    {
        var item = CriarItem(estoqueInicial: 2.5m, unidade: UnidadeMedida.Litro);

        Assert.Throws<InvalidOperationException>(() =>
            item.Atualizar("Óleo", null, UnidadeMedida.Unidade, 10));
        Assert.Equal(UnidadeMedida.Litro, item.UnidadeMedida);
    }

    // -------------------------------------------------------------------------
    // Entrada
    // -------------------------------------------------------------------------

    [Fact]
    public void AdicionarEstoque_ShouldIncreaseSaldo()
    {
        var item = CriarItem(estoqueInicial: 2);

        item.AdicionarEstoque(3);

        Assert.Equal(5, item.QuantidadeEmEstoque);
        Assert.Equal(5, item.QuantidadeDisponivel);
    }

    [Theory]
    [InlineData(UnidadeMedida.Litro)]
    [InlineData(UnidadeMedida.Quilo)]
    public void AdicionarEstoque_LitroOuQuilo_ShouldAcceptFracionado(UnidadeMedida unidade)
    {
        var item = CriarItem(estoqueInicial: 2, unidade: unidade);

        item.AdicionarEstoque(1.5m);

        Assert.Equal(3.5m, item.QuantidadeEmEstoque);
    }

    [Fact]
    public void AdicionarEstoque_UnidadeFracionada_ShouldThrowArgumentException()
    {
        var item = CriarItem();

        Assert.Throws<ArgumentException>(() => item.AdicionarEstoque(1.5m));
        Assert.Equal(0, item.QuantidadeEmEstoque);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void AdicionarEstoque_WithInvalidQuantidade_ShouldThrowArgumentException(decimal quantidade)
    {
        var item = CriarItem();

        Assert.Throws<ArgumentException>(() => item.AdicionarEstoque(quantidade));
    }

    // -------------------------------------------------------------------------
    // Reserva / liberação / baixa
    // -------------------------------------------------------------------------

    [Fact]
    public void Reservar_WithSaldo_ShouldReduceDisponivelButKeepFisico()
    {
        var item = CriarItem(estoqueInicial: 10);

        item.Reservar(4);

        Assert.Equal(10, item.QuantidadeEmEstoque);
        Assert.Equal(4, item.QuantidadeReservada);
        Assert.Equal(6, item.QuantidadeDisponivel);
        Assert.True(item.PossuiReserva());
    }

    [Fact]
    public void Reservar_SemSaldo_ShouldThrowAndNotChangeSaldo()
    {
        var item = CriarItem(estoqueInicial: 5);
        item.Reservar(3);

        Assert.Throws<InvalidOperationException>(() => item.Reservar(3));
        Assert.Equal(3, item.QuantidadeReservada);
    }

    [Fact]
    public void LiberarReserva_ShouldReturnQuantidadeToDisponivel()
    {
        var item = CriarItem(estoqueInicial: 10);
        item.Reservar(4);

        item.LiberarReserva(4);

        Assert.Equal(0, item.QuantidadeReservada);
        Assert.Equal(10, item.QuantidadeDisponivel);
        Assert.False(item.PossuiReserva());
    }

    [Fact]
    public void LiberarReserva_MaiorQueReservado_ShouldThrowInvalidOperationException()
    {
        var item = CriarItem(estoqueInicial: 10);
        item.Reservar(2);

        Assert.Throws<InvalidOperationException>(() => item.LiberarReserva(3));
    }

    [Fact]
    public void BaixarReserva_ShouldReduceFisicoAndReservado()
    {
        var item = CriarItem(estoqueInicial: 10);
        item.Reservar(4);

        item.BaixarReserva(4);

        Assert.Equal(6, item.QuantidadeEmEstoque);
        Assert.Equal(0, item.QuantidadeReservada);
        Assert.Equal(6, item.QuantidadeDisponivel);
    }

    [Fact]
    public void BaixarReserva_SemReserva_ShouldThrowInvalidOperationException()
    {
        var item = CriarItem(estoqueInicial: 10);

        Assert.Throws<InvalidOperationException>(() => item.BaixarReserva(1));
        Assert.Equal(10, item.QuantidadeEmEstoque);
    }
}
