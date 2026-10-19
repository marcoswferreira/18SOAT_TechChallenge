using Application.UseCases.OrdensServico;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using NSubstitute;

namespace Application.UnitTests.UseCases.OrdensServico;

public class GetOrdemServicoByIdUseCaseTests
{
    private readonly IOrdemServicoRepository _repository = Substitute.For<IOrdemServicoRepository>();
    private readonly GetOrdemServicoByIdUseCase _useCase;

    public GetOrdemServicoByIdUseCaseTests()
    {
        _useCase = new GetOrdemServicoByIdUseCase(_repository);
    }

    [Fact]
    public async Task ExecuteAsync_QuandoExiste_DeveRetornarOrdemServicoOutput()
    {
        // Arrange
        var os = new OrdemServico(Guid.NewGuid(), Guid.NewGuid());
        _repository.GetByIdWithDetailsAsync(os.Id, Arg.Any<CancellationToken>()).Returns(os);

        // Act
        var result = await _useCase.ExecuteAsync(os.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(os.Id, result.Id);
    }

    [Fact]
    public async Task ExecuteAsync_QuandoNaoExiste_DeveLancarKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repository.GetByIdWithDetailsAsync(id, Arg.Any<CancellationToken>()).Returns((OrdemServico?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _useCase.ExecuteAsync(id, CancellationToken.None));
    }
}

