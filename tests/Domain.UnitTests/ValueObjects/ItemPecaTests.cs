using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects;

public class ItemPecaTests
{
    [Fact]
    public void Criar_ComItemEstoqueValido_DeveInstanciarComSnapshotCorreto()
    {
        // Arrange
        var itemEstoque = new ItemEstoque("FLT-001", "Filtro de óleo", "Filtro 1.0", UnidadeMedida.Unidade, 45.00m);

        // Act
        var itemPeca = ItemPeca.Criar(itemEstoque, 2);

        // Assert
        Assert.Equal(itemEstoque.Id, itemPeca.ItemEstoqueId);
        Assert.Equal("Filtro de óleo", itemPeca.NomePeca);
        Assert.Equal(45.00m, itemPeca.ValorUnitario);
        Assert.Equal(2m, itemPeca.Quantidade);
        Assert.Equal(90.00m, itemPeca.ValorTotal);
    }

    [Fact]
    public void Criar_ComItemEstoqueLitro_DevePermitirQuantidadeFracionada()
    {
        // Arrange
        var oleo = new ItemEstoque("OLE-001", "Óleo 5W30", "Sintético", UnidadeMedida.Litro, 50.00m);

        // Act
        var itemPeca = ItemPeca.Criar(oleo, 3.5m);

        // Assert
        Assert.Equal(3.5m, itemPeca.Quantidade);
        Assert.Equal(175.00m, itemPeca.ValorTotal);
    }

    [Fact]
    public void Criar_ComItemEstoqueUnidade_QuantidadeFracionada_DeveLancarArgumentException()
    {
        // Arrange
        var vela = new ItemEstoque("VEL-001", "Vela de ignição", null, UnidadeMedida.Unidade, 25.00m);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => ItemPeca.Criar(vela, 1.5m));
        Assert.Contains("controlado por unidade", ex.Message);
    }

    [Fact]
    public void Criar_ComPrecoCustomizado_DevePrevalecerSobrePrecoDoEstoque()
    {
        // Arrange
        var pastilha = new ItemEstoque("PAS-001", "Pastilha", null, UnidadeMedida.Unidade, 100.00m);

        // Act
        var itemPeca = ItemPeca.Criar(pastilha, 1, valorUnitario: 80.00m);

        // Assert
        Assert.Equal(80.00m, itemPeca.ValorUnitario);
        Assert.Equal(80.00m, itemPeca.ValorTotal);
    }

    [Fact]
    public void Constructor_ComParametrosInvalidos_DeveLancarExcecoes()
    {
        Assert.Throws<ArgumentException>(() => new ItemPeca(Guid.Empty, "Peça", 10, 1));
        Assert.Throws<ArgumentException>(() => new ItemPeca(Guid.NewGuid(), "", 10, 1));
        Assert.Throws<ArgumentException>(() => new ItemPeca(Guid.NewGuid(), "Peça", -1, 1));
        Assert.Throws<ArgumentException>(() => new ItemPeca(Guid.NewGuid(), "Peça", 10, 0));
    }
}

