using Application.UseCases.Clientes;
using Application.UseCases.Clientes.Dto;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using NSubstitute;

namespace Application.UnitTests.UseCases.Clientes;

public class CreateClienteUseCaseTests
{
    private readonly IClienteRepository _clienteRepository = Substitute.For<IClienteRepository>();
    private readonly CreateClienteUseCase _useCase;

    private static EnderecoInput ValidEnderecoInput() => new(
        "Rua das Flores", "123", "Apto 1", "Bela Vista", "São Paulo", "SP", "01310-100");

    public CreateClienteUseCaseTests()
    {
        _useCase = new CreateClienteUseCase(_clienteRepository);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidInput_ShouldCreateClienteAndReturnSuccessResult()
    {
        var input = new CreateClienteInput("João Silva", "529.982.247-25", "joao@email.com", "(11) 99999-8888", ValidEnderecoInput());
        _clienteRepository.ExistsByDocumentoAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Errors);
        Assert.Equal("João Silva", result.Value.Nome);
        Assert.Equal("529.***.***-25", result.Value.CPFCNPJ);
        Assert.Equal("joao@email.com", result.Value.Email);
        Assert.Equal("(11) 99999-8888", result.Value.Telefone);
        Assert.Equal("Rua das Flores", result.Value.Endereco.Logradouro);
        Assert.Equal("SP", result.Value.Endereco.Estado);
        Assert.Equal("01310100", result.Value.Endereco.Cep);

        await _clienteRepository.Received(1).InsertAsync(
            Arg.Is<Cliente>(c => c.Nome == "João Silva" && c.Documento.Valor == "52998224725"),
            Arg.Any<CancellationToken>());
        await _clienteRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithMultipleInvalidFields_ShouldReturnFailureWithMultipleErrors()
    {
        var input = new CreateClienteInput("", "123", "email", "(11) 99999", null!);

        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.True(result.Errors.Count >= 3);
        Assert.Contains(result.Errors, e => e.Contains("nome"));
        Assert.Contains(result.Errors, e => e.Contains("documento"));
        Assert.Contains(result.Errors, e => e.Contains("endereço"));
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidEnderecoFields_ShouldReturnFailureWithMultipleAddressErrors()
    {
        var invalidEndereco = new EnderecoInput("", "", null, "", "", "XX", "123");
        var input = new CreateClienteInput("João", "529.982.247-25", null, null, invalidEndereco);

        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.True(result.Errors.Count >= 5);
        Assert.Contains(result.Errors, e => e.Contains("logradouro"));
        Assert.Contains(result.Errors, e => e.Contains("número"));
        Assert.Contains(result.Errors, e => e.Contains("bairro"));
        Assert.Contains(result.Errors, e => e.Contains("cidade"));
        Assert.Contains(result.Errors, e => e.Contains("estado"));
        Assert.Contains(result.Errors, e => e.Contains("CEP"));
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicateDocumento_ShouldReturnFailureResult()
    {
        var input = new CreateClienteInput("João Silva", "529.982.247-25", "joao@email.com", "123", ValidEnderecoInput());
        _clienteRepository.ExistsByDocumentoAsync("52998224725", Arg.Any<CancellationToken>())
            .Returns(true);

        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Contains("Já existe um cliente cadastrado", result.Errors[0]);
    }
}
