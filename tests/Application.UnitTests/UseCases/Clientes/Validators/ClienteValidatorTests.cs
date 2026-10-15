using Application.UseCases.Clientes.Dto;
using Application.UseCases.Clientes.Validators;

namespace Application.UnitTests.UseCases.Clientes.Validators;

public class ClienteValidatorTests
{
    private static EnderecoInput ValidEndereco() =>
        new("Av Paulista", "1000", null, "Bela Vista", "São Paulo", "SP", "01310100");

    [Fact]
    public void ValidateCreate_WhenValid_ShouldNotAddErrors()
    {
        var input = new CreateClienteInput("Carlos Silva", "52998224725", "carlos@email.com", "11999998888", ValidEndereco());
        var errors = new List<string>();

        ClienteValidator.ValidateCreate(input, errors);

        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateCreate_WhenEmptyNomeAndInvalidDocumento_ShouldAddErrors()
    {
        var input = new CreateClienteInput("", "111", null, null, ValidEndereco());
        var errors = new List<string>();

        ClienteValidator.ValidateCreate(input, errors);

        Assert.Contains("O nome do cliente é obrigatório.", errors);
        Assert.Contains("O documento informado é inválido. Informe um CPF ou CNPJ válido.", errors);
    }

    [Fact]
    public void ValidateCreate_WhenDocumentoEmpty_ShouldAddDocumentoRequiredError()
    {
        var input = new CreateClienteInput("Carlos", "", null, null, ValidEndereco());
        var errors = new List<string>();

        ClienteValidator.ValidateCreate(input, errors);

        Assert.Contains("O documento (CPF/CNPJ) é obrigatório.", errors);
    }

    [Fact]
    public void ValidateUpdate_WhenValid_ShouldNotAddErrors()
    {
        var input = new UpdateClienteInput("Carlos Silva", "carlos@email.com", "11999998888", ValidEndereco());
        var errors = new List<string>();

        ClienteValidator.ValidateUpdate(input, errors);

        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateUpdate_WhenEmptyNome_ShouldAddNomeRequiredError()
    {
        var input = new UpdateClienteInput("", null, null, ValidEndereco());
        var errors = new List<string>();

        ClienteValidator.ValidateUpdate(input, errors);

        Assert.Single(errors);
        Assert.Equal("O nome do cliente é obrigatório.", errors[0]);
    }
}

