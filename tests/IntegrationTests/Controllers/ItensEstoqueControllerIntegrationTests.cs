using System.Net;
using System.Net.Http.Json;
using Application.UseCases.ItensEstoque.Dto;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using IntegrationTests.Common;

namespace IntegrationTests.Controllers;

public class ItensEstoqueControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _authClient;
    private readonly HttpClient _anonymousClient;

    public ItensEstoqueControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _authClient = factory.CreateClientWithRole(Roles.Admin);
        _anonymousClient = factory.CreateClient();
    }

    [Fact]
    public async Task Create_ItemValido_DeveRetornar201CreatedComLocation()
    {
        // Arrange
        var codigo = $"PEC-{Guid.NewGuid():N}"[..10].ToUpperInvariant();
        var request = new CreateItemEstoqueInput(codigo, "Pastilha de Freio", "Pastilha dianteira", UnidadeMedida.Unidade, 150.00m);

        // Act
        var response = await _authClient.PostAsJsonAsync("/api/v1/ItensEstoque", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var content = await response.Content.ReadFromJsonAsync<ItemEstoqueOutput>();
        Assert.NotNull(content);
        Assert.Equal(codigo, content.Codigo);
    }

    [Fact]
    public async Task Create_Anonimo_DeveRetornar401Unauthorized()
    {
        // Arrange
        var request = new CreateItemEstoqueInput("COD123", "Filtro", null, UnidadeMedida.Unidade, 50.00m);

        // Act
        var response = await _anonymousClient.PostAsJsonAsync("/api/v1/ItensEstoque", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ItemExistente_DeveRetornar200OK()
    {
        // Arrange
        var item = new ItemEstoque("OLEO-5W30", "Óleo Sintético", "Lubrificante", UnidadeMedida.Litro, 45.00m);
        _factory.ItemEstoqueRepository.Seed(item);

        // Act
        var response = await _authClient.GetAsync($"/api/v1/ItensEstoque/{item.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ItemEstoqueOutput>();
        Assert.NotNull(content);
        Assert.Equal(item.Id, content.Id);
    }

    [Fact]
    public async Task GetById_ItemInexistente_DeveRetornar404NotFound()
    {
        // Act
        var response = await _authClient.GetAsync($"/api/v1/ItensEstoque/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPaged_DeveRetornar200OKComLista()
    {
        // Act
        var response = await _authClient.GetAsync("/api/v1/ItensEstoque?pageIndex=0&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_ItemExistente_DeveRetornar200OK()
    {
        // Arrange
        var item = new ItemEstoque("VELA-01", "Vela de Ignição", "Vela padrão", UnidadeMedida.Unidade, 30.00m);
        _factory.ItemEstoqueRepository.Seed(item);

        var updateRequest = new UpdateItemEstoqueInput("Vela Iridium", "Vela alta performance", UnidadeMedida.Unidade, 75.00m);

        // Act
        var response = await _authClient.PutAsJsonAsync($"/api/v1/ItensEstoque/{item.Id}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ItemEstoqueOutput>();
        Assert.NotNull(content);
        Assert.Equal("Vela Iridium", content.Nome);
    }

    [Fact]
    public async Task Update_ItemInexistente_DeveRetornar404NotFound()
    {
        // Arrange
        var updateRequest = new UpdateItemEstoqueInput("Nome", null, UnidadeMedida.Unidade, 10.00m);

        // Act
        var response = await _authClient.PutAsJsonAsync($"/api/v1/ItensEstoque/{Guid.NewGuid()}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ItemExistente_DeveRetornar204NoContent()
    {
        // Arrange
        var item = new ItemEstoque("FIL-AR", "Filtro de Ar", null, UnidadeMedida.Unidade, 25.00m);
        _factory.ItemEstoqueRepository.Seed(item);

        // Act
        var response = await _authClient.DeleteAsync($"/api/v1/ItensEstoque/{item.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ItemInexistente_DeveRetornar404NotFound()
    {
        // Act
        var response = await _authClient.DeleteAsync($"/api/v1/ItensEstoque/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RegistrarEntrada_ItemExistenteComQuantidadeValida_DeveRetornar200OK()
    {
        // Arrange
        var item = new ItemEstoque("AMORT-01", "Amortecedor", null, UnidadeMedida.Unidade, 400.00m);
        _factory.ItemEstoqueRepository.Seed(item);

        var request = new EntradaEstoqueInput(10);

        // Act
        var response = await _authClient.PostAsJsonAsync($"/api/v1/ItensEstoque/{item.Id}/entradas", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ItemEstoqueOutput>();
        Assert.NotNull(content);
        Assert.Equal(10, content.QuantidadeEmEstoque);
    }
}
