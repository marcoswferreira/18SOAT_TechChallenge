using System.Reflection;
using Application.UseCases.Users.Dto;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SIAES.API.Controllers;
using Xunit;

namespace IntegrationTests.Security;

/// <summary>
/// Garante que o autocadastro (POST /api/v1/Users) não permita escalação de privilégios,
/// e que o payload CreateUserInput não exponha campo Roles.
/// </summary>
public class UsersSecurityTests
{
    private static readonly Type ControllerType = typeof(UsersController);

    [Fact]
    public void CreateUserInput_NaoDeveConterPropriedadeRoles()
    {
        var rolesProperty = typeof(CreateUserInput).GetProperty("Roles", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        Assert.Null(rolesProperty);
    }

    [Fact]
    public void UsersController_Create_DeveSerAnonimo_MasReceberApenasCreateUserInput()
    {
        var createMethod = ControllerType.GetMethod(nameof(UsersController.Create));
        Assert.NotNull(createMethod);

        var allowAnonymous = createMethod.GetCustomAttribute<AllowAnonymousAttribute>();
        Assert.NotNull(allowAnonymous);

        var bodyParameter = createMethod.GetParameters()
            .FirstOrDefault(p => p.GetCustomAttribute<FromBodyAttribute>() is not null);

        Assert.NotNull(bodyParameter);
        Assert.Equal(typeof(CreateUserInput), bodyParameter.ParameterType);
    }

    [Fact]
    public void UsersController_OperacoesRestritas_DevemExigirAdmin()
    {
        var getPagedMethod = ControllerType.GetMethod(nameof(UsersController.GetPaged));
        Assert.NotNull(getPagedMethod);
        var getPagedAuth = getPagedMethod.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(getPagedAuth);
        Assert.Equal(Roles.Admin, getPagedAuth.Roles);

        var deleteMethod = ControllerType.GetMethod(nameof(UsersController.Delete));
        Assert.NotNull(deleteMethod);
        var deleteAuth = deleteMethod.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(deleteAuth);
        Assert.Equal(Roles.Admin, deleteAuth.Roles);
    }
}

