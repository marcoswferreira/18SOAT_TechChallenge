using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;

namespace IntegrationTests.Common;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public const string TestJwtSecret = "CHAVE_SUPER_SECRETA_E_LONGA_COM_PELO_MENOS_256_BITS";
    public const string TestIssuer = "SIAES.API";
    public const string TestAudience = "SIAES.Clients";

    public InMemoryClienteRepository ClienteRepository { get; } = new();
    public InMemoryVeiculoRepository VeiculoRepository { get; } = new();
    public InMemoryItemEstoqueRepository ItemEstoqueRepository { get; } = new();
    public InMemoryServicoRepository ServicoRepository { get; } = new();
    public InMemoryUserRepository UserRepository { get; } = new();
    public InMemoryOrdemServicoRepository OrdemServicoRepository { get; }

    public IOrdemServicoNotificationService NotificationServiceMock { get; } = Substitute.For<IOrdemServicoNotificationService>();
    public IResendEmailService ResendEmailServiceMock { get; } = Substitute.For<IResendEmailService>();

    public CustomWebApplicationFactory()
    {
        OrdemServicoRepository = new InMemoryOrdemServicoRepository(id => ClienteRepository.GetByIdAsync(id).Result);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseSetting("ConnectionStrings:DefaultConnection", "Host=localhost;Database=siaes_test;");
        builder.UseSetting("JwtSettings:Secret", TestJwtSecret);
        builder.UseSetting("JwtSettings:Issuer", TestIssuer);
        builder.UseSetting("JwtSettings:Audience", TestAudience);
        builder.UseSetting("JwtSettings:AccessTokenExpirationMinutes", "60");
        builder.UseSetting("JwtSettings:RefreshTokenExpirationDays", "7");
        builder.UseSetting("EncryptionSettings:Key", "CHAVE_SECRETA_AES_256_BITS_PARA_DADOS_SENSIVEIS_SIAES");
        builder.UseSetting("Resend:ApiToken", "re_test_dummy_token");
        builder.UseSetting("ResendSettings:ApiToken", "re_test_dummy_token");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=siaes_test;",
                ["JwtSettings:Secret"] = TestJwtSecret,
                ["JwtSettings:Issuer"] = TestIssuer,
                ["JwtSettings:Audience"] = TestAudience,
                ["JwtSettings:AccessTokenExpirationMinutes"] = "60",
                ["JwtSettings:RefreshTokenExpirationDays"] = "7",
                ["EncryptionSettings:Key"] = "CHAVE_SECRETA_AES_256_BITS_PARA_DADOS_SENSIVEIS_SIAES",
                ["Resend:ApiToken"] = "re_test_dummy_token",
                ["ResendSettings:ApiToken"] = "re_test_dummy_token"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IClienteRepository>();
            services.AddSingleton<IClienteRepository>(ClienteRepository);

            services.RemoveAll<IVeiculoRepository>();
            services.AddSingleton<IVeiculoRepository>(VeiculoRepository);

            services.RemoveAll<IItemEstoqueRepository>();
            services.AddSingleton<IItemEstoqueRepository>(ItemEstoqueRepository);

            services.RemoveAll<IServicoRepository>();
            services.AddSingleton<IServicoRepository>(ServicoRepository);

            services.RemoveAll<IUserRepository>();
            services.AddSingleton<IUserRepository>(UserRepository);

            services.RemoveAll<IOrdemServicoRepository>();
            services.AddSingleton<IOrdemServicoRepository>(OrdemServicoRepository);

            services.RemoveAll<IOrdemServicoNotificationService>();
            services.AddSingleton(NotificationServiceMock);

            services.RemoveAll<IResendEmailService>();
            services.AddSingleton(ResendEmailServiceMock);
        });
    }

    public void ResetRepositories()
    {
        ClienteRepository.Clear();
        VeiculoRepository.Clear();
        ItemEstoqueRepository.Clear();
        ServicoRepository.Clear();
        UserRepository.Clear();
        OrdemServicoRepository.Clear();
    }

    public HttpClient CreateClientWithRole(string role, Guid? userId = null)
    {
        var client = CreateClient();
        var token = GenerateAccessToken(userId ?? Guid.NewGuid(), "user@siaes.com", role);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public string GenerateAccessToken(Guid userId, string email, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestJwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new Dictionary<string, object>
        {
            [JwtRegisteredClaimNames.Sub] = userId.ToString(),
            [JwtRegisteredClaimNames.Email] = email,
            [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
            [ClaimTypes.Role] = role
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Claims = claims,
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = TestIssuer,
            Audience = TestAudience,
            SigningCredentials = creds
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}

