using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.UnitTests.Entities;

public class ClienteTests
{
    private static Documento ValidDocumento() => new("529.982.247-25");
    private static Endereco ValidEndereco() => new("Rua das Flores", "123", "Apto 1", "Centro", "São Paulo", "SP", "01001-000");

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateClienteAndNormalize()
    {
        var documento = ValidDocumento();
        var endereco = ValidEndereco();
        var cliente = new Cliente("  João da Silva  ", documento, "  JOAO@Email.COM  ", " (11) 99999-8888 ", endereco);

        Assert.NotEqual(Guid.Empty, cliente.Id);
        Assert.Equal("João da Silva", cliente.Nome);
        Assert.Equal(documento, cliente.Documento);
        Assert.Equal("joao@email.com", cliente.Email);
        Assert.Equal("(11) 99999-8888", cliente.Telefone);
        Assert.Equal(endereco, cliente.Endereco);
        Assert.Empty(cliente.Veiculos);
        Assert.False(cliente.IsDeleted);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidNome_ShouldThrowArgumentException(string nome)
    {
        Assert.Throws<ArgumentException>(() => new Cliente(nome, ValidDocumento(), "email@test.com", "123", ValidEndereco()));
    }

    [Fact]
    public void Constructor_WithNullDocumento_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Cliente("João", null!, "email@test.com", "123", ValidEndereco()));
    }

    [Fact]
    public void Constructor_WithNullEndereco_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Cliente("João", ValidDocumento(), "email@test.com", "123", null!));
    }

    [Fact]
    public void Update_WithValidParameters_ShouldUpdateFields()
    {
        var cliente = new Cliente("João", ValidDocumento(), "email@test.com", "123", ValidEndereco());
        var novoEndereco = new Endereco("Avenida Paulista", "1000", null, "Bela Vista", "São Paulo", "SP", "01310-100");

        cliente.Update("João Silva", "novo@email.com", "999", novoEndereco);

        Assert.Equal("João Silva", cliente.Nome);
        Assert.Equal("novo@email.com", cliente.Email);
        Assert.Equal("999", cliente.Telefone);
        Assert.Equal(novoEndereco, cliente.Endereco);
    }

    [Fact]
    public void UpdateEndereco_WithValidEndereco_ShouldUpdate()
    {
        var cliente = new Cliente("João", ValidDocumento(), null, null, ValidEndereco());
        var novoEndereco = new Endereco("Avenida Paulista", "1000", null, "Bela Vista", "São Paulo", "SP", "01310-100");

        cliente.UpdateEndereco(novoEndereco);

        Assert.Equal(novoEndereco, cliente.Endereco);
    }

    [Fact]
    public void UpdateDocumento_WithValidDocumento_ShouldUpdate()
    {
        var cliente = new Cliente("João", ValidDocumento(), null, null, ValidEndereco());
        var novoDoc = new Documento("11.222.333/0001-81");

        cliente.UpdateDocumento(novoDoc);

        Assert.Equal(novoDoc, cliente.Documento);
    }

    [Fact]
    public void AdicionarVeiculo_WithValidVeiculo_ShouldAddToCollection()
    {
        var cliente = new Cliente("João", ValidDocumento(), null, null, ValidEndereco());
        var veiculo = new Veiculo(cliente.Id, new Placa("ABC1234"), "Fiat", "Uno", 2010);

        cliente.AdicionarVeiculo(veiculo);

        Assert.Single(cliente.Veiculos);
        Assert.Contains(veiculo, cliente.Veiculos);
    }

    [Fact]
    public void AdicionarVeiculo_WithDuplicatePlaca_ShouldThrowInvalidOperationException()
    {
        var cliente = new Cliente("João", ValidDocumento(), null, null, ValidEndereco());
        var veiculo1 = new Veiculo(cliente.Id, new Placa("ABC1234"), "Fiat", "Uno", 2010);
        var veiculo2 = new Veiculo(cliente.Id, new Placa("ABC1234"), "Ford", "Ka", 2015);

        cliente.AdicionarVeiculo(veiculo1);

        var ex = Assert.Throws<InvalidOperationException>(() => cliente.AdicionarVeiculo(veiculo2));
        Assert.Contains("já está associado", ex.Message);
    }

    [Fact]
    public void RemoverVeiculo_WithExistingVeiculo_ShouldRemove()
    {
        var cliente = new Cliente("João", ValidDocumento(), null, null, ValidEndereco());
        var veiculo = new Veiculo(cliente.Id, new Placa("ABC1234"), "Fiat", "Uno", 2010);

        cliente.AdicionarVeiculo(veiculo);
        cliente.RemoverVeiculo(veiculo.Id);

        Assert.Empty(cliente.Veiculos);
    }
}
