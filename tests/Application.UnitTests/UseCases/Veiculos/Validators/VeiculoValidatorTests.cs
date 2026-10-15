using Application.UseCases.Veiculos.Dto;
using Application.UseCases.Veiculos.Validators;

namespace Application.UnitTests.UseCases.Veiculos.Validators;

public class VeiculoValidatorTests
{
    [Fact]
    public void ValidateCreate_WhenValid_ShouldNotAddErrors()
    {
        var input = new CreateVeiculoInput(Guid.NewGuid(), "BRA2E19", "Toyota", "Corolla", 2022);
        var errors = new List<string>();

        VeiculoValidator.ValidateCreate(input, errors);

        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateCreate_WhenEmptyClienteId_ShouldAddClienteIdError()
    {
        var input = new CreateVeiculoInput(Guid.Empty, "BRA2E19", "Toyota", "Corolla", 2022);
        var errors = new List<string>();

        VeiculoValidator.ValidateCreate(input, errors);

        Assert.Single(errors);
        Assert.Equal("O identificador do cliente é obrigatório.", errors[0]);
    }

    [Fact]
    public void ValidateUpdate_WhenValid_ShouldNotAddErrors()
    {
        var input = new UpdateVeiculoInput("BRA2E19", "Toyota", "Corolla", 2022);
        var errors = new List<string>();

        VeiculoValidator.ValidateUpdate(input, errors);

        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateDadosVeiculo_WhenEmptyOrInvalidFields_ShouldAddRespectiveErrors()
    {
        var input = new UpdateVeiculoInput("INVALIDA", "", "", 1800);
        var errors = new List<string>();

        VeiculoValidator.ValidateUpdate(input, errors);

        Assert.Equal(4, errors.Count);
        Assert.Contains("A placa informada é inválida.", errors);
        Assert.Contains("A marca do veículo é obrigatória.", errors);
        Assert.Contains("O modelo do veículo é obrigatório.", errors);
        Assert.Contains(errors, e => e.Contains("O ano do veículo deve ser entre 1900"));
    }

    [Fact]
    public void ValidateDadosVeiculo_WhenPlacaEmpty_ShouldAddPlacaRequiredError()
    {
        var input = new UpdateVeiculoInput("", "Toyota", "Corolla", 2022);
        var errors = new List<string>();

        VeiculoValidator.ValidateUpdate(input, errors);

        Assert.Single(errors);
        Assert.Equal("A placa do veículo é obrigatória.", errors[0]);
    }
}

