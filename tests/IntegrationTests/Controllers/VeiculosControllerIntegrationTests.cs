using System.Net;
using System.Net.Http.Json;
using Application.UseCases.Veiculos.Dto;
using Domain.Constants;
using Domain.Entities;
using Domain.ValueObjects;
using IntegrationTests.Common;

namespace IntegrationTests.Controllers;

public class VeiculosControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _authClient;
    private readonly HttpClient _anonymousClient;

    public VeiculosControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _authClient = factory.CreateClientWithRole(Roles.Admin);
        _anonymousClient = factory.CreateClient();
    }

    [Fact]
    public async Task Create_VeiculoValido_DeveRetornar201CreatedComLocation()
    {
        // Arrange
        var cliente = new Cliente("Proprietario", new Documento("52998224725"), "p@email.com", null, new Endereco("Rua", "1", null, "B", "C", "SP", "01001000"));
        _factory.ClienteRepository.Seed(cliente);

        var request = new CreateVeiculoInput(cliente.Id, "ABC1D23", "Toyota", "Corolla", 2022);

        // Act
        var response = await _authClient.PostAsJsonAsync("/api/v1/Veiculos", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var content = await response.Content.ReadFromJsonAsync<VeiculoOutput>();
        Assert.NotNull(content);
        Assert.Equal("ABC***3", content.Placa);
    }

    [Fact]
    public async Task Create_ComPlacaInvalida_DeveRetornar400BadRequest()
    {
        // Arrange
        var request = new CreateVeiculoInput(Guid.NewGuid(), "INVALIDA", "Toyota", "Corolla", 2022);

        // Act
        var response = await _authClient.PostAsJsonAsync("/api/v1/Veiculos", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_Anonimo_DeveRetornar401Unauthorized()
    {
        // Arrange
        var request = new CreateVeiculoInput(Guid.NewGuid(), "ABC1D23", "Toyota", "Corolla", 2022);

        // Act
        var response = await _anonymousClient.PostAsJsonAsync("/api/v1/Veiculos", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetById_VeiculoExistente_DeveRetornar200OK()
    {
        // Arrange
        var veiculo = new Veiculo(Guid.NewGuid(), new Placa("BRA2E19"), "Honda", "Civic", 2021);
        _factory.VeiculoRepository.Seed(veiculo);

        // Act
        var response = await _authClient.GetAsync($"/api/v1/Veiculos/{veiculo.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<VeiculoOutput>();
        Assert.NotNull(content);
        Assert.Equal(veiculo.Id, content.Id);
    }

    [Fact]
    public async Task GetById_VeiculoInexistente_DeveRetornar404NotFound()
    {
        // Act
        var response = await _authClient.GetAsync($"/api/v1/Veiculos/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPaged_DeveRetornar200OKComLista()
    {
        // Act
        var response = await _authClient.GetAsync("/api/v1/Veiculos?pageIndex=0&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_VeiculoExistente_DeveRetornar200OK()
    {
        // Arrange
        var veiculo = new Veiculo(Guid.NewGuid(), new Placa("XYZ9A87"), "Ford", "Ka", 2020);
        _factory.VeiculoRepository.Seed(veiculo);

        var updateRequest = new UpdateVeiculoInput("XYZ9A87", "Ford", "Ka Sedan", 2020);

        // Act
        var response = await _authClient.PutAsJsonAsync($"/api/v1/Veiculos/{veiculo.Id}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<VeiculoOutput>();
        Assert.NotNull(content);
        Assert.Equal("Ka Sedan", content.Modelo);
    }

    [Fact]
    public async Task Update_VeiculoInexistente_DeveRetornar404NotFound()
    {
        // Arrange
        var updateRequest = new UpdateVeiculoInput("XYZ9A87", "Ford", "Ka", 2020);

        // Act
        var response = await _authClient.PutAsJsonAsync($"/api/v1/Veiculos/{Guid.NewGuid()}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_VeiculoExistente_DeveRetornar204NoContent()
    {
        // Arrange
        var veiculo = new Veiculo(Guid.NewGuid(), new Placa("KTM4F56"), "VW", "Gol", 2019);
        _factory.VeiculoRepository.Seed(veiculo);

        // Act
        var response = await _authClient.DeleteAsync($"/api/v1/Veiculos/{veiculo.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_VeiculoInexistente_DeveRetornar404NotFound()
    {
        // Act
        var response = await _authClient.DeleteAsync($"/api/v1/Veiculos/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

