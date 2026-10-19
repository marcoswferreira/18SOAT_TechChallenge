using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects;

public class ItemServicoTests
{
    [Fact]
    public void Criar_ComServicoValido_DeveInstanciarComSnapshotCorreto()
    {
        // Arrange
        var servico = new Servico("Alinhamento e Balanceamento");

        // Act
        var itemServico = ItemServico.Criar(servico, 120.00m, 1);

        // Assert
        Assert.Equal(servico.Id, itemServico.ServicoId);
        Assert.Equal("Alinhamento e Balanceamento", itemServico.Descricao);
        Assert.Equal(120.00m, itemServico.ValorUnitario);
        Assert.Equal(1, itemServico.Quantidade);
        Assert.Equal(120.00m, itemServico.ValorTotal);
    }

    [Fact]
    public void Constructor_ComParametrosInvalidos_DeveLancarExcecoes()
    {
        Assert.Throws<ArgumentException>(() => new ItemServico(null, "", 100m, 1));
        Assert.Throws<ArgumentException>(() => new ItemServico(null, "Troca de correia", -10m, 1));
        Assert.Throws<ArgumentException>(() => new ItemServico(null, "Troca de correia", 100m, 0));
    }
}

