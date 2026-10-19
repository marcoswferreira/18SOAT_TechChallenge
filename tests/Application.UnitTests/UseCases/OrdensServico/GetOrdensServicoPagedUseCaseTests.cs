using Application.UseCases.OrdensServico;
using Domain.Common.Paginate;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using NSubstitute;

namespace Application.UnitTests.UseCases.OrdensServico;

public class GetOrdensServicoPagedUseCaseTests
{
    private readonly IOrdemServicoRepository _repository = Substitute.For<IOrdemServicoRepository>();
    private readonly GetOrdensServicoPagedUseCase _useCase;

    public GetOrdensServicoPagedUseCaseTests()
    {
        _useCase = new GetOrdensServicoPagedUseCase(_repository);
    }

    [Fact]
    public async Task ExecuteAsync_DeveRetornarItensPaginados()
    {
        // Arrange
        var os1 = new OrdemServico(Guid.NewGuid(), Guid.NewGuid());
        var os2 = new OrdemServico(Guid.NewGuid(), Guid.NewGuid());
        var pagedList = Substitute.For<IPaginate<OrdemServico>>();
        pagedList.Items.Returns([os1, os2]);
        pagedList.Count.Returns(2);

        _repository.GetPagedAsync(0, 10, Arg.Any<CancellationToken>()).Returns(pagedList);

        // Act
        var result = await _useCase.ExecuteAsync(0, 10, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
    }
}
