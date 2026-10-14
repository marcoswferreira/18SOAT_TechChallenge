using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.UnitTests.Entities;

public class VeiculoTests
{
    private static Placa ValidPlaca() => new("BRA2E19");

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateVeiculo()
    {
        var clienteId = Guid.NewGuid();
        var placa = ValidPlaca();
        var veiculo = new Veiculo(clienteId, placa, "  Toyota  ", "  Corolla  ", 2022);

        Assert.NotEqual(Guid.Empty, veiculo.Id);
        Assert.Equal(clienteId, veiculo.ClienteId);
        Assert.Equal(placa, veiculo.Placa);
        Assert.Equal("Toyota", veiculo.Marca);
        Assert.Equal("Corolla", veiculo.Modelo);
        Assert.Equal(2022, veiculo.Ano);
        Assert.False(veiculo.IsDeleted);
    }

    [Fact]
    public void Constructor_WithEmptyClienteId_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Veiculo(Guid.Empty, ValidPlaca(), "Toyota", "Corolla", 2022));
    }

    [Fact]
    public void Constructor_WithNullPlaca_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Veiculo(Guid.NewGuid(), null!, "Toyota", "Corolla", 2022));
    }

    [Theory]
    [InlineData("", "Corolla")]
    [InlineData("   ", "Corolla")]
    [InlineData("Toyota", "")]
    [InlineData("Toyota", "   ")]
    public void Constructor_WithInvalidMarcaOrModelo_ShouldThrowArgumentException(string marca, string modelo)
    {
        Assert.Throws<ArgumentException>(() => new Veiculo(Guid.NewGuid(), ValidPlaca(), marca, modelo, 2022));
    }

    [Theory]
    [InlineData(1899)]
    [InlineData(2050)]
    public void Constructor_WithInvalidAno_ShouldThrowArgumentOutOfRangeException(int ano)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Veiculo(Guid.NewGuid(), ValidPlaca(), "Toyota", "Corolla", ano));
    }

    [Fact]
    public void Update_WithValidParameters_ShouldUpdateFields()
    {
        var veiculo = new Veiculo(Guid.NewGuid(), ValidPlaca(), "Toyota", "Corolla", 2022);
        var novaPlaca = new Placa("ABC1234");

        veiculo.Update(novaPlaca, "Honda", "Civic", 2023);

        Assert.Equal(novaPlaca, veiculo.Placa);
        Assert.Equal("Honda", veiculo.Marca);
        Assert.Equal("Civic", veiculo.Modelo);
        Assert.Equal(2023, veiculo.Ano);
    }

    [Fact]
    public void TransferirParaCliente_WithValidClienteId_ShouldUpdateClienteId()
    {
        var veiculo = new Veiculo(Guid.NewGuid(), ValidPlaca(), "Toyota", "Corolla", 2022);
        var novoClienteId = Guid.NewGuid();

        veiculo.TransferirParaCliente(novoClienteId);

        Assert.Equal(novoClienteId, veiculo.ClienteId);
        Assert.Null(veiculo.Cliente);
    }

    [Fact]
    public void TransferirParaCliente_WithEmptyGuid_ShouldThrowArgumentException()
    {
        var veiculo = new Veiculo(Guid.NewGuid(), ValidPlaca(), "Toyota", "Corolla", 2022);

        Assert.Throws<ArgumentException>(() => veiculo.TransferirParaCliente(Guid.Empty));
    }
}

