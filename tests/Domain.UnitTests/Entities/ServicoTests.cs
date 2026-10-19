using Domain.Entities;

namespace Domain.UnitTests.Entities;

public class ServicoTests
{
    [Fact]
    public void Constructor_WithValidName_ShouldCreateServicoAndNormalize()
    {
        var servico = new Servico("  Lavagem  ");

        Assert.NotEqual(Guid.Empty, servico.Id);
        Assert.Equal("Lavagem", servico.Nome);
        Assert.False(servico.IsDeleted);
        Assert.True(servico.CreatedAt <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidName_ShouldThrowArgumentException(string nome)
    {
        Assert.Throws<ArgumentException>(() => new Servico(nome));
    }

    [Fact]
    public void Update_WithValidName_ShouldUpdateAndTrim()
    {
        var servico = new Servico("Antiga");

        servico.Update("  Nova  ");

        Assert.Equal("Nova", servico.Nome);
    }

    [Fact]
    public void Update_WithInvalidName_ShouldThrowArgumentException()
    {
        var servico = new Servico("X");

        Assert.Throws<ArgumentException>(() => servico.Update("  "));
    }

    [Fact]
    public void DeleteAndRestore_ShouldToggleIsDeletedAndSetMetadata()
    {
        var servico = new Servico("ToDelete");

        servico.Delete("tester");

        Assert.True(servico.IsDeleted);
        Assert.Equal("tester", servico.DeletedBy);
        Assert.NotNull(servico.DeletedAt);

        servico.Restore();

        Assert.False(servico.IsDeleted);
        Assert.Null(servico.DeletedBy);
        Assert.Null(servico.DeletedAt);
    }
}
