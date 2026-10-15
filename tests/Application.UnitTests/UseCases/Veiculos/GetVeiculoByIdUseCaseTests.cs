using Application.UseCases.Veiculos;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;
using NSubstitute;

namespace Application.UnitTests.UseCases.Veiculos;

public class GetVeiculoByIdUseCaseTests
{
    private readonly IVeiculoRepository _veiculoRepository = Substitute.For<IVeiculoRepository>();
    private readonly GetVeiculoByIdUseCase _useCase;

    public GetVeiculoByIdUseCaseTests()
    {
        _useCase = new GetVeiculoByIdUseCase(_veiculoRepository);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingId_ShouldReturnVeiculoOutput()
    {
        var id = Guid.NewGuid();
        var veiculo = new Veiculo(Guid.NewGuid(), new Placa("BRA2E19"), "Toyota", "Corolla", 2022);
        _veiculoRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(veiculo);

        var result = await _useCase.ExecuteAsync(id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("BRA***9", result.Placa);
        Assert.Equal("Toyota", result.Marca);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistingId_ShouldThrowKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _veiculoRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Veiculo?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _useCase.ExecuteAsync(id, CancellationToken.None));
    }
}

