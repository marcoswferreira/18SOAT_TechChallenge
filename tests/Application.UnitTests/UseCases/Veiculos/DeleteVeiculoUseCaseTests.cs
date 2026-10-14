using Application.UseCases.Veiculos;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;
using NSubstitute;

namespace Application.UnitTests.UseCases.Veiculos;

public class DeleteVeiculoUseCaseTests
{
    private readonly IVeiculoRepository _veiculoRepository = Substitute.For<IVeiculoRepository>();
    private readonly DeleteVeiculoUseCase _useCase;

    public DeleteVeiculoUseCaseTests()
    {
        _useCase = new DeleteVeiculoUseCase(_veiculoRepository);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingId_ShouldCallDeleteAsyncAndReturnSuccess()
    {
        var id = Guid.NewGuid();
        var veiculo = new Veiculo(Guid.NewGuid(), new Placa("BRA2E19"), "Toyota", "Corolla", 2022);
        _veiculoRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(veiculo);

        var result = await _useCase.ExecuteAsync(id, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
        _veiculoRepository.Received(1).Delete(veiculo);
        await _veiculoRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistingId_ShouldThrowKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _veiculoRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Veiculo?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _useCase.ExecuteAsync(id, CancellationToken.None));
    }
}
