using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects;

public class EnderecoTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateAndFormat()
    {
        var endereco = new Endereco(
            "  Avenida Paulista  ",
            "  1000  ",
            "  Apto 42  ",
            "  Bela Vista  ",
            "  São Paulo  ",
            "  sp  ",
            "  01310-100  "
        );

        Assert.Equal("Avenida Paulista", endereco.Logradouro);
        Assert.Equal("1000", endereco.Numero);
        Assert.Equal("Apto 42", endereco.Complemento);
        Assert.Equal("Bela Vista", endereco.Bairro);
        Assert.Equal("São Paulo", endereco.Cidade);
        Assert.Equal("SP", endereco.Estado);
        Assert.Equal("01310100", endereco.Cep);
        Assert.Equal("01310-100", endereco.CepFormatado);
        Assert.Equal("Avenida Paulista, 1000 - Apto 42, Bela Vista, São Paulo - SP, CEP: 01310-100", endereco.Formatado);
        Assert.Equal(endereco.Formatado, endereco.ToString());
    }

    [Fact]
    public void Constructor_WithoutComplemento_ShouldFormatWithoutComplemento()
    {
        var endereco = new Endereco("Rua das Flores", "123", null, "Centro", "Curitiba", "PR", "80010-000");

        Assert.Null(endereco.Complemento);
        Assert.Equal("Rua das Flores, 123, Centro, Curitiba - PR, CEP: 80010-000", endereco.Formatado);
    }

    [Theory]
    [InlineData("XX")]
    [InlineData("ZZ")]
    [InlineData("SPO")]
    public void Constructor_WithInvalidEstado_ShouldThrowArgumentException(string estado)
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new Endereco("Rua A", "1", null, "Bairro", "Cidade", estado, "01001-000"));
        Assert.Contains("Estado", ex.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyEstado_ShouldThrowArgumentException(string estado)
    {
        Assert.Throws<ArgumentException>(() =>
            new Endereco("Rua A", "1", null, "Bairro", "Cidade", estado, "01001-000"));
    }

    [Theory]
    [InlineData("1234567")]
    [InlineData("123456789")]
    [InlineData("ABCDE-FGH")]
    public void Constructor_WithInvalidCep_ShouldThrowArgumentException(string cep)
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new Endereco("Rua A", "1", null, "Bairro", "Cidade", "SP", cep));
        Assert.Contains("CEP", ex.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyCep_ShouldThrowArgumentException(string cep)
    {
        Assert.Throws<ArgumentException>(() =>
            new Endereco("Rua A", "1", null, "Bairro", "Cidade", "SP", cep));
    }

    [Theory]
    [InlineData("", "1", "Bairro", "Cidade")]
    [InlineData("Rua", "", "Bairro", "Cidade")]
    [InlineData("Rua", "1", "", "Cidade")]
    [InlineData("Rua", "1", "Bairro", "")]
    public void Constructor_WithEmptyRequiredFields_ShouldThrowArgumentException(
        string logradouro, string numero, string bairro, string cidade)
    {
        Assert.Throws<ArgumentException>(() =>
            new Endereco(logradouro, numero, null, bairro, cidade, "SP", "01001-000"));
    }

    [Fact]
    public void Equals_WithSameValues_ShouldBeEqual()
    {
        var end1 = new Endereco("Rua A", "10", "Apto 1", "Centro", "Campinas", "SP", "13010-000");
        var end2 = new Endereco("Rua A", "10", "Apto 1", "Centro", "Campinas", "sp", "13010000");

        Assert.Equal(end1, end2);
        Assert.True(end1 == end2);
    }
}
