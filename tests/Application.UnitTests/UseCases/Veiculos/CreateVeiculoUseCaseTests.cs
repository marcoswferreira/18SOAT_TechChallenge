using Application.UseCases.Veiculos;
using Application.UseCases.Veiculos.Dto;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;
using NSubstitute;

namespace Application.UnitTests.UseCases.Veiculos;

public class CreateVeiculoUseCaseTests
{
    private readonly IVeiculoRepository _veiculoRepository = Substitute.For<IVeiculoRepository>();
    private readonly IClienteRepository _clienteRepository = Substitute.For<IClienteRepository>();
    private readonly CreateVeiculoUseCase _useCase;

    private static Endereco ValidEndereco() => new("Rua A", "10", null, "Centro", "São Paulo", "SP", "01001-000");

    public CreateVeiculoUseCaseTests()
    {
        _useCase = new CreateVeiculoUseCase(_veiculoRepository, _clienteRepository);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidInput_ShouldCreateVeiculoAndReturnSuccessResult()
    {
        var clienteId = Guid.NewGuid();
        var cliente = new Cliente("João", new Documento("529.982.247-25"), null, null, ValidEndereco());
        _clienteRepository.GetByIdAsync(clienteId, Arg.Any<CancellationToken>()).Returns(cliente);
        _veiculoRepository.ExistsByPlacaAsync("BRA2E19", Arg.Any<CancellationToken>()).Returns(false);

        var input = new CreateVeiculoInput(clienteId, "BRA2E19", "Toyota", "Corolla", 2022);

        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(clienteId, result.Value.ClienteId);
        Assert.Equal("BRA***9", result.Value.Placa);
        Assert.Equal("Toyota", result.Value.Marca);
        Assert.Equal("Corolla", result.Value.Modelo);
        Assert.Equal(2022, result.Value.Ano);

        await _veiculoRepository.Received(1).InsertAsync(
            Arg.Is<Veiculo>(v => v.Placa.Valor == "BRA2E19" && v.ClienteId == clienteId),
            Arg.Any<CancellationToken>());
        await _veiculoRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithMultipleInvalidFields_ShouldReturnFailureWithMultipleErrors()
    {
        var input = new CreateVeiculoInput(Guid.Empty, "INVALIDA", "", "", 1800);

        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.True(result.Errors.Count >= 5);
        Assert.Contains(result.Errors, e => e.Contains("identificador do cliente"));
        Assert.Contains(result.Errors, e => e.Contains("placa"));
        Assert.Contains(result.Errors, e => e.Contains("marca"));
        Assert.Contains(result.Errors, e => e.Contains("modelo"));
        Assert.Contains(result.Errors, e => e.Contains("ano"));
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistingCliente_ShouldReturnFailureResult()
    {
        var clienteId = Guid.NewGuid();
        _clienteRepository.GetByIdAsync(clienteId, Arg.Any<CancellationToken>()).Returns((Cliente?)null);

        var input = new CreateVeiculoInput(clienteId, "BRA2E19", "Toyota", "Corolla", 2022);

        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Contains("não foi encontrado"));
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicatePlaca_ShouldReturnFailureResult()
    {
        var clienteId = Guid.NewGuid();
        var cliente = new Cliente("João", new Documento("529.982.247-25"), null, null, ValidEndereco());
        _clienteRepository.GetByIdAsync(clienteId, Arg.Any<CancellationToken>()).Returns(cliente);
        _veiculoRepository.ExistsByPlacaAsync("BRA2E19", Arg.Any<CancellationToken>()).Returns(true);

        var input = new CreateVeiculoInput(clienteId, "BRA2E19", "Toyota", "Corolla", 2022);

        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Contains("Já existe um veículo cadastrado"));
    }
}
