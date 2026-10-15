using Application.UseCases.Clientes.Dto;
using Application.UseCases.Clientes.Validators;

namespace Application.UnitTests.UseCases.Clientes.Validators;

public class EnderecoValidatorTests
{
    [Fact]
    public void Validate_WhenNull_ShouldAddRequiredError()
    {
        var errors = new List<string>();
        EnderecoValidator.Validate(null, errors);

        Assert.Single(errors);
        Assert.Equal("O endereço é obrigatório.", errors[0]);
    }

    [Fact]
    public void Validate_WhenValid_ShouldNotAddErrors()
    {
        var endereco = new EnderecoInput("Av Paulista", "1000", "Apto 1", "Bela Vista", "São Paulo", "SP", "01310100");
        var errors = new List<string>();

        EnderecoValidator.Validate(endereco, errors);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_WhenFieldsAreEmptyOrInvalid_ShouldAddRespectiveErrors()
    {
        var endereco = new EnderecoInput("", "", null, "", "", "XX", "123");
        var errors = new List<string>();

        EnderecoValidator.Validate(endereco, errors);

        Assert.Contains("O logradouro do endereço é obrigatório.", errors);
        Assert.Contains("O número do endereço é obrigatório.", errors);
        Assert.Contains("O bairro do endereço é obrigatório.", errors);
        Assert.Contains("A cidade do endereço é obrigatória.", errors);
        Assert.Contains("O estado (UF) 'XX' é inválido.", errors);
        Assert.Contains("O CEP deve conter exatamente 8 dígitos numéricos.", errors);
    }

    [Fact]
    public void Validate_WhenEstadoAndCepEmpty_ShouldAddMandatoryErrors()
    {
        var endereco = new EnderecoInput("Rua A", "1", null, "Bairro", "Cidade", "", "");
        var errors = new List<string>();

        EnderecoValidator.Validate(endereco, errors);

        Assert.Contains("O estado (UF) do endereço é obrigatório.", errors);
        Assert.Contains("O CEP do endereço é obrigatório.", errors);
    }
}

