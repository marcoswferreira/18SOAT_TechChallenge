using Application.UseCases.Clientes;
using Application.UseCases.Clientes.Dto;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;
using NSubstitute;

namespace Application.UnitTests.UseCases.Clientes;

public class UpdateClienteUseCaseTests
{
    private readonly IClienteRepository _clienteRepository = Substitute.For<IClienteRepository>();
    private readonly UpdateClienteUseCase _useCase;

    private static Endereco ValidEndereco() => new("Rua Antiga", "10", null, "Centro", "São Paulo", "SP", "01001-000");
    private static EnderecoInput ValidEnderecoInput() => new("Rua Nova", "20", null, "Bela Vista", "São Paulo", "SP", "01310-100");

    public UpdateClienteUseCaseTests()
    {
        _useCase = new UpdateClienteUseCase(_clienteRepository);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingCliente_ShouldUpdateAndReturnSuccessResult()
    {
        var id = Guid.NewGuid();
        var existingCliente = new Cliente("João Antigo", new Documento("529.982.247-25"), "velho@email.com", "111", ValidEndereco());
        _clienteRepository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(existingCliente);

        var input = new UpdateClienteInput("João Novo", "novo@email.com", "222", ValidEnderecoInput());

        var result = await _useCase.ExecuteAsync(id, input, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("João Novo", result.Value.Nome);
        Assert.Equal("novo@email.com", result.Value.Email);
        Assert.Equal("222", result.Value.Telefone);
        Assert.Equal("Rua Nova", result.Value.Endereco.Logradouro);
        Assert.Equal("20", result.Value.Endereco.Numero);

        _clienteRepository.Received(1).Update(existingCliente);
        await _clienteRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithMultipleInvalidFields_ShouldReturnFailureWithMultipleErrors()
    {
        var id = Guid.NewGuid();
        var existingCliente = new Cliente("João Antigo", new Documento("529.982.247-25"), null, null, ValidEndereco());
        _clienteRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(existingCliente);

        var invalidEndereco = new EnderecoInput("", "", null, "", "", "XX", "123");
        var input = new UpdateClienteInput("", "novo@email.com", "222", invalidEndereco);

        var result = await _useCase.ExecuteAsync(id, input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.True(result.Errors.Count >= 5);
        Assert.Contains(result.Errors, e => e.Contains("nome"));
        Assert.Contains(result.Errors, e => e.Contains("logradouro"));
        Assert.Contains(result.Errors, e => e.Contains("número"));
        Assert.Contains(result.Errors, e => e.Contains("estado"));
        Assert.Contains(result.Errors, e => e.Contains("CEP"));
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistingCliente_ShouldThrowKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _clienteRepository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns((Cliente?)null);

        var input = new UpdateClienteInput("João Novo", "novo@email.com", "222", ValidEnderecoInput());

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _useCase.ExecuteAsync(id, input, CancellationToken.None));
    }
}
