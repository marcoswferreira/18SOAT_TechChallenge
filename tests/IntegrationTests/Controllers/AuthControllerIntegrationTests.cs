using System.Net;
using System.Net.Http.Json;
using Application.UseCases.Auth.Dto;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using IntegrationTests.Common;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Controllers;

public class AuthControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _passwordHasher = factory.Services.GetRequiredService<IPasswordHasher>();
        _tokenService = factory.Services.GetRequiredService<ITokenService>();
    }

    [Fact]
    public async Task Login_ComCredenciaisValidas_DeveRetornar200ETokenComCookie()
    {
        // Arrange
        var email = $"login-{Guid.NewGuid():N}@siaes.com";
        var password = "SenhaValida123!";
        var passwordHash = _passwordHasher.Hash(password);
        var user = new User(email, passwordHash, [Roles.Admin]);
        _factory.UserRepository.Seed(user);

        var request = new AuthInput(email, password);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(content);
        Assert.False(string.IsNullOrWhiteSpace(content.AccessToken));

        var cookies = response.Headers.GetValues("Set-Cookie");
        Assert.Contains(cookies, c => c.Contains("refreshToken="));
    }

    [Fact]
    public async Task Login_ComCredenciaisInvalidas_DeveRetornar401()
    {
        // Arrange
        var request = new AuthInput("inexistente@siaes.com", "SenhaErrada1!");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_ComTokenValido_DeveRetornar200ENovoAccessToken()
    {
        // Arrange
        var email = $"refresh-{Guid.NewGuid():N}@siaes.com";
        var rawToken = _tokenService.GenerateRefreshToken();
        var hashedToken = _tokenService.HashToken(rawToken);

        var user = new User(email, "hash", [Roles.Admin]);
        user.SetRefreshToken(hashedToken, TimeSpan.FromDays(7));
        _factory.UserRepository.Seed(user);

        var request = new RefreshTokenRequest(rawToken);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/refresh-token", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(content);
        Assert.False(string.IsNullOrWhiteSpace(content.AccessToken));
    }

    [Fact]
    public async Task RefreshToken_SemToken_DeveRetornar401()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/refresh-token", new RefreshTokenRequest(null));

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_ComToken_DeveRetornar204ERemoverCookie()
    {
        // Arrange
        var rawToken = _tokenService.GenerateRefreshToken();
        var request = new RefreshTokenRequest(rawToken);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/logout", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private sealed record LoginResponse(string AccessToken);
}
