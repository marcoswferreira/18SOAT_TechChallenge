using Application.UseCases.OrdensServico;
using Application.UseCases.OrdensServico.Dto;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;
using Domain.Interfaces.Services;
using NSubstitute;

namespace Application.UnitTests.UseCases.OrdensServico;

public class CreateOrdemServicoUseCaseTests
{
    private readonly IOrdemServicoRepository _ordemServicoRepository = Substitute.For<IOrdemServicoRepository>();
    private readonly IClienteRepository _clienteRepository = Substitute.For<IClienteRepository>();
    private readonly IVeiculoRepository _veiculoRepository = Substitute.For<IVeiculoRepository>();
    private readonly IOrdemServicoNotificationService _notificationService = Substitute.For<IOrdemServicoNotificationService>();
    private readonly CreateOrdemServicoUseCase _useCase;

    private static Endereco ValidEndereco() => new("Rua B", "123", null, "Centro", "São Paulo", "SP", "01001-000");

    public CreateOrdemServicoUseCaseTests()
    {
        _useCase = new CreateOrdemServicoUseCase(_ordemServicoRepository, _clienteRepository, _veiculoRepository, _notificationService);
    }

    [Fact]
    public async Task ExecuteAsync_QuandoClienteEVeiculoExistem_DeveCriarOrdemServico()
    {
        // Arrange
        var cliente = new Cliente("Maria", new Documento("529.982.247-25"), "maria@email.com", "123", ValidEndereco());
        var veiculo = new Veiculo(cliente.Id, new Placa("ABC1234"), "Fiat", "Uno", 2020);
        var input = new CreateOrdemServicoInput(cliente.Id, veiculo.Id);

        _clienteRepository.GetByIdAsync(cliente.Id, Arg.Any<CancellationToken>()).Returns(cliente);
        _veiculoRepository.GetByIdAsync(veiculo.Id, Arg.Any<CancellationToken>()).Returns(veiculo);

        // Act
        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cliente.Id, result.ClienteId);
        Assert.Equal(veiculo.Id, result.VeiculoId);
        Assert.Equal(StatusOrdemServico.Recebida.ToString(), result.Status);

        await _ordemServicoRepository.Received(1).InsertAsync(Arg.Any<OrdemServico>(), Arg.Any<CancellationToken>());
        await _ordemServicoRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _notificationService.Received(1).NotificarOrdemServicoCriadaAsync(Arg.Any<OrdemServico>(), cliente, veiculo, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_QuandoClienteNaoExiste_DeveLancarKeyNotFoundException()
    {
        // Arrange
        var input = new CreateOrdemServicoInput(Guid.NewGuid(), Guid.NewGuid());
        _clienteRepository.GetByIdAsync(input.ClienteId, Arg.Any<CancellationToken>()).Returns((Cliente?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _useCase.ExecuteAsync(input, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_QuandoVeiculoNaoExiste_DeveLancarKeyNotFoundException()
    {
        // Arrange
        var cliente = new Cliente("Maria", new Documento("529.982.247-25"), "maria@email.com", "123", ValidEndereco());
        var input = new CreateOrdemServicoInput(cliente.Id, Guid.NewGuid());

        _clienteRepository.GetByIdAsync(cliente.Id, Arg.Any<CancellationToken>()).Returns(cliente);
        _veiculoRepository.GetByIdAsync(input.VeiculoId, Arg.Any<CancellationToken>()).Returns((Veiculo?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _useCase.ExecuteAsync(input, CancellationToken.None));
    }
}
