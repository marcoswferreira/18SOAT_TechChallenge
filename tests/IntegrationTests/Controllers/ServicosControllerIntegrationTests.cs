using System.Net;
using System.Net.Http.Json;
using Application.UseCases.Servicos.Dto;
using Domain.Constants;
using Domain.Entities;
using IntegrationTests.Common;

namespace IntegrationTests.Controllers;

public class ServicosControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _authClient;
    private readonly HttpClient _anonymousClient;

    public ServicosControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _authClient = factory.CreateClientWithRole(Roles.Admin);
        _anonymousClient = factory.CreateClient();
    }

    [Fact]
    public async Task Create_ServicoValido_DeveRetornar201CreatedComLocation()
    {
        // Arrange
        var request = new CreateServicoInput("Alinhamento 3D");

        // Act
        var response = await _authClient.PostAsJsonAsync("/api/v1/Servicos", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var content = await response.Content.ReadFromJsonAsync<ServicoOutput>();
        Assert.NotNull(content);
        Assert.Equal("Alinhamento 3D", content.Nome);
    }

    [Fact]
    public async Task Create_Anonimo_DeveRetornar401Unauthorized()
    {
        // Arrange
        var request = new CreateServicoInput("Balanceamento");

        // Act
        var response = await _anonymousClient.PostAsJsonAsync("/api/v1/Servicos", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ServicoExistente_DeveRetornar200OK()
    {
        // Arrange
        var servico = new Servico("Troca de Óleo");
        _factory.ServicoRepository.Seed(servico);

        // Act
        var response = await _authClient.GetAsync($"/api/v1/Servicos/{servico.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ServicoOutput>();
        Assert.NotNull(content);
        Assert.Equal(servico.Id, content.Id);
    }

    [Fact]
    public async Task GetById_ServicoInexistente_DeveRetornar400BadRequest()
    {
        // Act
        var response = await _authClient.GetAsync($"/api/v1/Servicos/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetPaged_DeveRetornar200OKComLista()
    {
        // Act
        var response = await _authClient.GetAsync("/api/v1/Servicos?pageIndex=0&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_ServicoExistente_DeveRetornar200OK()
    {
        // Arrange
        var servico = new Servico("Revisão Básica");
        _factory.ServicoRepository.Seed(servico);

        var updateRequest = new UpdateServicoInput(servico.Id, "Revisão Completa");

        // Act
        var response = await _authClient.PutAsJsonAsync($"/api/v1/Servicos/{servico.Id}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ServicoOutput>();
        Assert.NotNull(content);
        Assert.Equal("Revisão Completa", content.Nome);
    }

    [Fact]
    public async Task Update_ServicoInexistente_DeveRetornar404NotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateRequest = new UpdateServicoInput(id, "Serviço Inexistente");

        // Act
        var response = await _authClient.PutAsJsonAsync($"/api/v1/Servicos/{id}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ServicoExistente_DeveRetornar204NoContent()
    {
        // Arrange
        var servico = new Servico("Higienização de Ar");
        _factory.ServicoRepository.Seed(servico);

        // Act
        var response = await _authClient.DeleteAsync($"/api/v1/Servicos/{servico.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ServicoInexistente_DeveRetornar400BadRequest()
    {
        // Act
        var response = await _authClient.DeleteAsync($"/api/v1/Servicos/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

