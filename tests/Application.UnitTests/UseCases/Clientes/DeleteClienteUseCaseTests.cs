using Application.UseCases.Clientes;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;
using NSubstitute;

namespace Application.UnitTests.UseCases.Clientes;

public class DeleteClienteUseCaseTests
{
    private readonly IClienteRepository _clienteRepository = Substitute.For<IClienteRepository>();
    private readonly DeleteClienteUseCase _useCase;

    private static Endereco ValidEndereco() => new("Rua B", "123", null, "Centro", "São Paulo", "SP", "01001-000");

    public DeleteClienteUseCaseTests()
    {
        _useCase = new DeleteClienteUseCase(_clienteRepository);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingIdAndNoVeiculos_ShouldCallDeleteAsyncAndReturnSuccess()
    {
        var id = Guid.NewGuid();
        var cliente = new Cliente("Maria", new Documento("529.982.247-25"), null, null, ValidEndereco());
        _clienteRepository.GetByIdWithVeiculosAsync(id, Arg.Any<CancellationToken>()).Returns(cliente);

        var result = await _useCase.ExecuteAsync(id, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
        _clienteRepository.Received(1).Delete(cliente);
        await _clienteRepository.SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingClienteHavingVeiculos_ShouldReturnFailureResult()
    {
        var id = Guid.NewGuid();
        var cliente = new Cliente("Maria", new Documento("529.982.247-25"), null, null, ValidEndereco());
        cliente.AdicionarVeiculo(new Veiculo(cliente.Id, new Placa("ABC1234"), "Fiat", "Uno", 2010));
        _clienteRepository.GetByIdWithVeiculosAsync(id, Arg.Any<CancellationToken>()).Returns(cliente);

        var result = await _useCase.ExecuteAsync(id, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Contains("possui 1 veículo(s)"));
        _clienteRepository.DidNotReceive().Delete(cliente);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistingId_ShouldThrowKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _clienteRepository.GetByIdWithVeiculosAsync(id, Arg.Any<CancellationToken>()).Returns((Cliente?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _useCase.ExecuteAsync(id, CancellationToken.None));
    }
}
