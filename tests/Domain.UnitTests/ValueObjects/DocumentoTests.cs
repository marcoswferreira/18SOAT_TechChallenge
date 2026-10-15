using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects;

public class DocumentoTests
{
    [Theory]
    [InlineData("52998224725", "52998224725", "529.982.247-25", TipoDocumento.CPF)]
    [InlineData("529.982.247-25", "52998224725", "529.982.247-25", TipoDocumento.CPF)]
    [InlineData("11222333000181", "11222333000181", "11.222.333/0001-81", TipoDocumento.CNPJ)]
    [InlineData("11.222.333/0001-81", "11222333000181", "11.222.333/0001-81", TipoDocumento.CNPJ)]
    public void Constructor_WithValidDocumento_ShouldCreateInstanceAndFormat(
        string input, string expectedValor, string expectedFormatado, TipoDocumento expectedTipo)
    {
        var documento = new Documento(input);

        Assert.Equal(expectedValor, documento.Valor);
        Assert.Equal(expectedFormatado, documento.Formatado);
        Assert.Equal(expectedTipo, documento.Tipo);
        Assert.Equal(expectedValor, documento.ToString());
    }

    [Theory]
    [InlineData("52998224725", "529.***.***-25")]
    [InlineData("529.982.247-25", "529.***.***-25")]
    [InlineData("11222333000181", "11.***.***/****-81")]
    [InlineData("11.222.333/0001-81", "11.***.***/****-81")]
    public void Mascarado_ShouldObfuscateSensitiveDocumentData(string input, string expectedMascarado)
    {
        var documento = new Documento(input);
        Assert.Equal(expectedMascarado, documento.Mascarado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrWhitespace_ShouldThrowArgumentException(string input)
    {
        var exception = Assert.Throws<ArgumentException>(() => new Documento(input));
        Assert.Contains("obrigatório", exception.Message);
    }

    [Theory]
    [InlineData("11111111111")] // dígitos repetidos
    [InlineData("00000000000")]
    [InlineData("12345678901")] // dígitos verificadores inválidos
    [InlineData("123")]
    [InlineData("123456789012")]
    [InlineData("00000000000000")] // CNPJ repetido
    [InlineData("11222333000199")] // CNPJ com digito inválido
    public void Constructor_WithInvalidDocumento_ShouldThrowArgumentException(string input)
    {
        var exception = Assert.Throws<ArgumentException>(() => new Documento(input));
        Assert.Contains("inválido", exception.Message);
    }

    [Fact]
    public void Equals_WithSameValues_ShouldBeEqual()
    {
        var doc1 = new Documento("529.982.247-25");
        var doc2 = new Documento("52998224725");

        Assert.Equal(doc1, doc2);
        Assert.True(doc1 == doc2);
    }
}
