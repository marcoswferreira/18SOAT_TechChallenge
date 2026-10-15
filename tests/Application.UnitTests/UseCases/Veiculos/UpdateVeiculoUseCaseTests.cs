using Application.UseCases.Veiculos;
using Application.UseCases.Veiculos.Dto;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;
using NSubstitute;

namespace Application.UnitTests.UseCases.Veiculos;

public class UpdateVeiculoUseCaseTests
{
    private readonly IVeiculoRepository _veiculoRepository = Substitute.For<IVeiculoRepository>();
    private readonly UpdateVeiculoUseCase _useCase;

    public UpdateVeiculoUseCaseTests()
    {
        _useCase = new UpdateVeiculoUseCase(_veiculoRepository);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingVeiculoAndSamePlaca_ShouldUpdateFieldsAndReturnSuccessResult()
    {
        var id = Guid.NewGuid();
        var veiculo = new Veiculo(Guid.NewGuid(), new Placa("BRA2E19"), "Toyota", "Corolla", 2020);
        _veiculoRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(veiculo);

        var input = new UpdateVeiculoInput("BRA2E19", "Toyota", "Corolla XEi", 2021);

        var result = await _useCase.ExecuteAsync(id, input, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Corolla XEi", result.Value.Modelo);
        Assert.Equal(2021, result.Value.Ano);
        _veiculoRepository.Received(1).Update(veiculo);
        await _veiculoRepository.SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithMultipleInvalidFields_ShouldReturnFailureWithMultipleErrors()
    {
        var id = Guid.NewGuid();
        var veiculo = new Veiculo(Guid.NewGuid(), new Placa("BRA2E19"), "Toyota", "Corolla", 2020);
        _veiculoRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(veiculo);

        var input = new UpdateVeiculoInput("INVALIDA", "", "", 1800);

        var result = await _useCase.ExecuteAsync(id, input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.True(result.Errors.Count >= 4);
        Assert.Contains(result.Errors, e => e.Contains("placa"));
        Assert.Contains(result.Errors, e => e.Contains("marca"));
        Assert.Contains(result.Errors, e => e.Contains("modelo"));
        Assert.Contains(result.Errors, e => e.Contains("ano"));
    }

    [Fact]
    public async Task ExecuteAsync_WithPlacaChangedToDuplicate_ShouldReturnFailureResult()
    {
        var id = Guid.NewGuid();
        var veiculo = new Veiculo(Guid.NewGuid(), new Placa("BRA2E19"), "Toyota", "Corolla", 2020);
        _veiculoRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(veiculo);
        _veiculoRepository.ExistsByPlacaAsync("ABC1234", Arg.Any<CancellationToken>()).Returns(true);

        var input = new UpdateVeiculoInput("ABC1234", "Toyota", "Corolla", 2020);

        var result = await _useCase.ExecuteAsync(id, input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Já existe um veículo cadastrado", result.Errors[0]);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistingVeiculo_ShouldThrowKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _veiculoRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Veiculo?)null);

        var input = new UpdateVeiculoInput("ABC1234", "Toyota", "Corolla", 2020);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _useCase.ExecuteAsync(id, input, CancellationToken.None));
    }
}
