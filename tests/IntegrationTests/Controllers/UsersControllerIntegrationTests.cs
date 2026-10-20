using System.Net;
using System.Net.Http.Json;
using Application.UseCases.Users.Dto;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using IntegrationTests.Common;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Controllers;

public class UsersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _adminClient;
    private readonly HttpClient _anonymousClient;
    private readonly IPasswordHasher _passwordHasher;

    public UsersControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _adminClient = factory.CreateClientWithRole(Roles.Admin);
        _anonymousClient = factory.CreateClient();
        _passwordHasher = factory.Services.GetRequiredService<IPasswordHasher>();
    }

    [Fact]
    public async Task Create_NovoUsuarioAutocadastro_DeveRetornar201CreatedComLocation()
    {
        // Arrange
        var email = $"novo-{Guid.NewGuid():N}@siaes.com";
        var request = new CreateUserInput(email, "SenhaForte123!");

        // Act
        var response = await _anonymousClient.PostAsJsonAsync("/api/v1/Users", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var content = await response.Content.ReadFromJsonAsync<UserOutput>();
        Assert.NotNull(content);
        Assert.Equal(email, content.Email);
        Assert.Contains(Roles.User, content.Roles);
    }

    [Fact]
    public async Task GetById_UsuarioExistente_DeveRetornar200OK()
    {
        // Arrange
        var email = $"user-{Guid.NewGuid():N}@siaes.com";
        var user = new User(email, _passwordHasher.Hash("Senha123!"), [Roles.User]);
        _factory.UserRepository.Seed(user);

        // Act
        var response = await _adminClient.GetAsync($"/api/v1/Users/{user.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<UserOutput>();
        Assert.NotNull(content);
        Assert.Equal(user.Id, content.Id);
    }

    [Fact]
    public async Task GetById_UsuarioInexistente_DeveRetornar404NotFound()
    {
        // Act
        var response = await _adminClient.GetAsync($"/api/v1/Users/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPaged_DeveRetornar200OKComLista()
    {
        // Act
        var response = await _adminClient.GetAsync("/api/v1/Users?pageIndex=0&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_UsuarioExistente_DeveRetornar200OK()
    {
        // Arrange
        var email = $"update-{Guid.NewGuid():N}@siaes.com";
        var user = new User(email, _passwordHasher.Hash("Senha123!"), [Roles.User]);
        _factory.UserRepository.Seed(user);

        var updatedEmail = $"updated-{Guid.NewGuid():N}@siaes.com";
        var updateRequest = new UpdateUserInput(updatedEmail, [Roles.Operacional]);

        // Act
        var response = await _adminClient.PutAsJsonAsync($"/api/v1/Users/{user.Id}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<UserOutput>();
        Assert.NotNull(content);
        Assert.Equal(updatedEmail, content.Email);
        Assert.Contains(Roles.Operacional, content.Roles);
    }

    [Fact]
    public async Task Update_UsuarioInexistente_DeveRetornar404NotFound()
    {
        // Arrange
        var updateRequest = new UpdateUserInput("inexistente@siaes.com", [Roles.User]);

        // Act
        var response = await _adminClient.PutAsJsonAsync($"/api/v1/Users/{Guid.NewGuid()}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_ComSenhaCorreta_DeveRetornar204NoContent()
    {
        // Arrange
        var email = $"pwd-{Guid.NewGuid():N}@siaes.com";
        var currentPassword = "SenhaAtual123!";
        var user = new User(email, _passwordHasher.Hash(currentPassword), [Roles.User]);
        _factory.UserRepository.Seed(user);

        var userClient = _factory.CreateClientWithRole(Roles.User, user.Id);
        var request = new ChangePasswordInput(currentPassword, "NovaSenha456!");

        // Act
        var response = await userClient.PutAsJsonAsync($"/api/v1/Users/{user.Id}/password", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_ComSenhaAtualIncorreta_DeveRetornar401Unauthorized()
    {
        // Arrange
        var email = $"pwd-err-{Guid.NewGuid():N}@siaes.com";
        var user = new User(email, _passwordHasher.Hash("SenhaCorreta123!"), [Roles.User]);
        _factory.UserRepository.Seed(user);

        var userClient = _factory.CreateClientWithRole(Roles.User, user.Id);
        var request = new ChangePasswordInput("SenhaErrada!", "NovaSenha456!");

        // Act
        var response = await userClient.PutAsJsonAsync($"/api/v1/Users/{user.Id}/password", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_UsuarioExistente_DeveRetornar204NoContent()
    {
        // Arrange
        var email = $"del-{Guid.NewGuid():N}@siaes.com";
        var user = new User(email, _passwordHasher.Hash("Senha123!"), [Roles.User]);
        _factory.UserRepository.Seed(user);

        // Act
        var response = await _adminClient.DeleteAsync($"/api/v1/Users/{user.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_UsuarioInexistente_DeveRetornar404NotFound()
    {
        // Act
        var response = await _adminClient.DeleteAsync($"/api/v1/Users/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

