using System.Net;
using System.Net.Http.Json;
using Application.UseCases.Clientes.Dto;
using Domain.Constants;
using Domain.Entities;
using Domain.ValueObjects;
using IntegrationTests.Common;

namespace IntegrationTests.Controllers;

public class ClientesControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _authClient;
    private readonly HttpClient _anonymousClient;

    public ClientesControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _authClient = factory.CreateClientWithRole(Roles.Admin);
        _anonymousClient = factory.CreateClient();
    }

    private static EnderecoInput CriarEnderecoInput() =>
        new("Rua das Oficinas", "123", "Sala 1", "Centro", "São Paulo", "SP", "01001-000");

    [Fact]
    public async Task Create_ClienteValido_DeveRetornar201CreatedComLocation()
    {
        // Arrange - CNPJ válido para evitar colisão com seeds de CPF em outros testes
        var cnpj = "11222333000181";
        var request = new CreateClienteInput("Empresa Silva LTDA", cnpj, "contato@empresa.com", "11999998888", CriarEnderecoInput());

        // Act
        var response = await _authClient.PostAsJsonAsync("/api/v1/Clientes", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var content = await response.Content.ReadFromJsonAsync<ClienteOutput>();
        Assert.NotNull(content);
        Assert.Equal("Empresa Silva LTDA", content.Nome);
    }

    [Fact]
    public async Task Create_ClienteComCpfInvalido_DeveRetornar400BadRequest()
    {
        // Arrange
        var request = new CreateClienteInput("Invalido", "00000000000", "inv@email.com", "11999998888", CriarEnderecoInput());

        // Act
        var response = await _authClient.PostAsJsonAsync("/api/v1/Clientes", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_Anonimo_DeveRetornar401Unauthorized()
    {
        // Arrange
        var request = new CreateClienteInput("Anon", "52998224725", "anon@email.com", null, CriarEnderecoInput());

        // Act
        var response = await _anonymousClient.PostAsJsonAsync("/api/v1/Clientes", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ClienteExistente_DeveRetornar200OK()
    {
        // Arrange
        var doc = new Documento("52998224725");
        var endereco = new Endereco("Rua A", "10", null, "Bairro", "Cidade", "SP", "01001000");
        var cliente = new Cliente("Maria Souza", doc, "maria@email.com", "11988887777", endereco);
        _factory.ClienteRepository.Seed(cliente);

        // Act
        var response = await _authClient.GetAsync($"/api/v1/Clientes/{cliente.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ClienteOutput>();
        Assert.NotNull(content);
        Assert.Equal(cliente.Id, content.Id);
    }

    [Fact]
    public async Task GetById_ClienteInexistente_DeveRetornar404NotFound()
    {
        // Act
        var response = await _authClient.GetAsync($"/api/v1/Clientes/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPaged_DeveRetornar200OKComLista()
    {
        // Act
        var response = await _authClient.GetAsync("/api/v1/Clientes?pageIndex=0&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_ClienteExistente_DeveRetornar200OK()
    {
        // Arrange
        var doc = new Documento("52998224725");
        var endereco = new Endereco("Rua B", "20", null, "Bairro", "Cidade", "SP", "01001000");
        var cliente = new Cliente("Carlos Lima", doc, "carlos@email.com", "11977776666", endereco);
        _factory.ClienteRepository.Seed(cliente);

        var updateRequest = new UpdateClienteInput("Carlos Lima Atualizado", "carlos.novo@email.com", "11977776666", CriarEnderecoInput());

        // Act
        var response = await _authClient.PutAsJsonAsync($"/api/v1/Clientes/{cliente.Id}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ClienteOutput>();
        Assert.NotNull(content);
        Assert.Equal("Carlos Lima Atualizado", content.Nome);
    }

    [Fact]
    public async Task Update_ClienteInexistente_DeveRetornar404NotFound()
    {
        // Arrange
        var updateRequest = new UpdateClienteInput("Nome", "email@email.com", null, CriarEnderecoInput());

        // Act
        var response = await _authClient.PutAsJsonAsync($"/api/v1/Clientes/{Guid.NewGuid()}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ClienteExistente_DeveRetornar204NoContent()
    {
        // Arrange
        var doc = new Documento("52998224725");
        var endereco = new Endereco("Rua C", "30", null, "Bairro", "Cidade", "SP", "01001000");
        var cliente = new Cliente("Jose Silva", doc, "jose@email.com", "11966665555", endereco);
        _factory.ClienteRepository.Seed(cliente);

        // Act
        var response = await _authClient.DeleteAsync($"/api/v1/Clientes/{cliente.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ClienteInexistente_DeveRetornar404NotFound()
    {
        // Act
        var response = await _authClient.DeleteAsync($"/api/v1/Clientes/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

