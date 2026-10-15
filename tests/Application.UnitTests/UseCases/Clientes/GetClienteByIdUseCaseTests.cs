using Application.UseCases.Clientes;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;
using NSubstitute;

namespace Application.UnitTests.UseCases.Clientes;

public class GetClienteByIdUseCaseTests
{
    private readonly IClienteRepository _clienteRepository = Substitute.For<IClienteRepository>();
    private readonly GetClienteByIdUseCase _useCase;

    private static Endereco ValidEndereco() => new("Rua B", "123", null, "Centro", "São Paulo", "SP", "01001-000");

    public GetClienteByIdUseCaseTests()
    {
        _useCase = new GetClienteByIdUseCase(_clienteRepository);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingId_ShouldReturnClienteOutput()
    {
        var id = Guid.NewGuid();
        var cliente = new Cliente("Maria", new Documento("529.982.247-25"), "maria@email.com", "123", ValidEndereco());
        _clienteRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(cliente);

        var result = await _useCase.ExecuteAsync(id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Maria", result.Nome);
        Assert.Equal("529.***.***-25", result.CPFCNPJ);
        Assert.Equal("Rua B", result.Endereco.Logradouro);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistingId_ShouldThrowKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _clienteRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Cliente?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _useCase.ExecuteAsync(id, CancellationToken.None));
    }
}
