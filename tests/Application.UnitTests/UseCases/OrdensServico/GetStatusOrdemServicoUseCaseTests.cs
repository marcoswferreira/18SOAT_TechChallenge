using Application.UseCases.OrdensServico;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;
using NSubstitute;

namespace Application.UnitTests.UseCases.OrdensServico;

public class GetStatusOrdemServicoUseCaseTests
{
    private readonly IOrdemServicoRepository _repository = Substitute.For<IOrdemServicoRepository>();
    private readonly GetStatusOrdemServicoUseCase _useCase;

    public GetStatusOrdemServicoUseCaseTests()
    {
        _useCase = new GetStatusOrdemServicoUseCase(_repository);
    }

    [Fact]
    public async Task ExecuteAsync_PorId_QuandoExistir_DeveRetornarGetStatusClienteResponse()
    {
        // Arrange
        var os = new OrdemServico(Guid.NewGuid(), Guid.NewGuid());
        os.AdicionarServico(new ItemServico("Troca de óleo", 100m, 1));
        os.AdicionarPeca(new ItemPeca("Filtro", 50m, 1));
        _repository.GetByIdAsync(os.Id, Arg.Any<CancellationToken>()).Returns(os);

        // Act
        var result = await _useCase.ExecuteAsync(os.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(os.Id, result.OrdemServicoId);
        Assert.Equal(StatusOrdemServico.Recebida.ToString(), result.Status);
        Assert.Equal(150m, result.ValorTotal);
        Assert.Equal(1, result.TotalServicos);
        Assert.Equal(1, result.TotalPecas);
    }

    [Fact]
    public async Task ExecuteAsync_PorId_QuandoNaoExistir_DeveLancarKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((OrdemServico?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _useCase.ExecuteAsync(id, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_PorDocumento_QuandoValido_DeveRetornarListaDeStatus()
    {
        // Arrange
        var cpfValido = "529.982.247-25";
        var os1 = new OrdemServico(Guid.NewGuid(), Guid.NewGuid());
        var os2 = new OrdemServico(Guid.NewGuid(), Guid.NewGuid());
        _repository.GetByDocumentoClienteAsync(Arg.Any<Documento>(), Arg.Any<CancellationToken>())
            .Returns([os1, os2]);

        // Act
        var result = (await _useCase.ExecuteAsync(cpfValido, CancellationToken.None)).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(os1.Id, result[0].OrdemServicoId);
        Assert.Equal(os2.Id, result[1].OrdemServicoId);
    }

    [Fact]
    public async Task ExecuteAsync_PorDocumento_QuandoInvalido_DeveLancarArgumentException()
    {
        // Arrange
        var documentoInvalido = "123456";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _useCase.ExecuteAsync(documentoInvalido, CancellationToken.None));
    }
}
