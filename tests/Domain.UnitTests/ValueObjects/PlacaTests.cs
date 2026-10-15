using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects;

public class PlacaTests
{
    [Theory]
    [InlineData("ABC1234", "ABC1234", "ABC-1234", false)]
    [InlineData("abc-1234", "ABC1234", "ABC-1234", false)]
    [InlineData("  ABC1D23  ", "ABC1D23", "ABC1D23", true)]
    [InlineData("BRA2E19", "BRA2E19", "BRA2E19", true)]
    public void Constructor_WithValidPlaca_ShouldCreateInstanceAndFormat(
        string input, string expectedValor, string expectedFormatado, bool expectedMercosul)
    {
        var placa = new Placa(input);

        Assert.Equal(expectedValor, placa.Valor);
        Assert.Equal(expectedFormatado, placa.Formatado);
        Assert.Equal(expectedMercosul, placa.EhMercosul);
        Assert.Equal(expectedValor, placa.ToString());
    }

    [Theory]
    [InlineData("BRA2E19", "BRA***9")]
    [InlineData("abc-1234", "ABC***4")]
    [InlineData("ABC1234", "ABC***4")]
    public void Mascarada_ShouldObfuscateSensitivePlateData(string input, string expectedMascarada)
    {
        var placa = new Placa(input);
        Assert.Equal(expectedMascarada, placa.Mascarada);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrWhitespace_ShouldThrowArgumentException(string input)
    {
        var exception = Assert.Throws<ArgumentException>(() => new Placa(input));
        Assert.Contains("obrigatória", exception.Message);
    }

    [Theory]
    [InlineData("1234ABC")]
    [InlineData("ABCD123")]
    [InlineData("ABC12345")]
    [InlineData("AB1234")]
    [InlineData("ABC-1D2")]
    public void Constructor_WithInvalidPlaca_ShouldThrowArgumentException(string input)
    {
        var exception = Assert.Throws<ArgumentException>(() => new Placa(input));
        Assert.Contains("inválida", exception.Message);
    }

    [Fact]
    public void Equals_WithSameValues_ShouldBeEqual()
    {
        var placa1 = new Placa("abc-1234");
        var placa2 = new Placa("ABC1234");

        Assert.Equal(placa1, placa2);
        Assert.True(placa1 == placa2);
    }
}
