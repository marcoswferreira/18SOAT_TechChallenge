using System.Reflection;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using SIAES.API.Controllers;
using Xunit;

namespace IntegrationTests.Security;

/// <summary>
/// Trava a política de acesso das Ordens de Serviço: tudo exige autenticação e perfil operacional,
/// exceto as duas consultas públicas de status. Se alguém abrir um endpoint por engano, estes testes falham.
/// </summary>
public class OrdensServicoAuthorizationTests
{
    private static readonly Type ControllerType = typeof(OrdensServicoController);

    private static List<MethodInfo> Actions() => ControllerType
        .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
        .Where(m => m.GetCustomAttributes<HttpMethodAttribute>().Any())
        .ToList();

    [Fact]
    public void Controller_DeveExigirAutenticacaoEPerfilOperacionalPorPadrao()
    {
        var authorize = ControllerType.GetCustomAttribute<AuthorizeAttribute>();

        Assert.NotNull(authorize);
        Assert.False(string.IsNullOrWhiteSpace(authorize.Roles));

        var perfis = authorize.Roles!.Split(',', StringSplitOptions.TrimEntries);
        Assert.Contains(Roles.Admin, perfis);
        Assert.Contains(Roles.Manager, perfis);
        Assert.Contains(Roles.Operacional, perfis);
    }

    [Fact]
    public void Controller_NaoDevePermitirPerfilUser()
    {
        var authorize = ControllerType.GetCustomAttribute<AuthorizeAttribute>();

        var perfis = authorize!.Roles!.Split(',', StringSplitOptions.TrimEntries);

        Assert.DoesNotContain(Roles.User, perfis);
    }

    [Fact]
    public void ApenasAsDuasConsultasDeStatusDevemSerAnonimas()
    {
        var acoes = Actions();
        Assert.NotEmpty(acoes);

        var anonimas = acoes
            .Where(m => m.GetCustomAttribute<AllowAnonymousAttribute>() is not null)
            .Select(m => m.Name)
            .Order()
            .ToArray();

        var esperadas = new[]
        {
            nameof(OrdensServicoController.ObterStatusPorId),
            nameof(OrdensServicoController.ObterStatusPorDocumento)
        }.Order().ToArray();

        Assert.Equal(esperadas, anonimas);
    }

    [Fact]
    public void AcoesQueAlteramEstado_NuncaDevemSerAnonimas()
    {
        var escritas = Actions()
            .Where(m => m.GetCustomAttributes<HttpMethodAttribute>()
                .Any(a => a.HttpMethods.Any(v => v != HttpMethods.Get)));

        Assert.NotEmpty(escritas);
        Assert.All(escritas, m => Assert.Null(m.GetCustomAttribute<AllowAnonymousAttribute>()));
    }
}
