using Application.UseCases.Auth;
using Application.UseCases.Clientes;
using Application.UseCases.Users;
using Application.UseCases.Veiculos;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Auth Use Cases
        services.AddScoped<AuthenticateUserUseCase>();
        services.AddScoped<RefreshTokenUseCase>();
        services.AddScoped<RevokeTokenUseCase>();

        // User Use Cases
        services.AddScoped<CreateUserUseCase>();
        services.AddScoped<GetUserByIdUseCase>();
        services.AddScoped<GetUsersPagedUseCase>();
        services.AddScoped<UpdateUserUseCase>();
        services.AddScoped<DeleteUserUseCase>();
        services.AddScoped<ChangePasswordUseCase>();

        // Cliente Use Cases
        services.AddScoped<CreateClienteUseCase>();
        services.AddScoped<GetClienteByIdUseCase>();
        services.AddScoped<GetClientesPagedUseCase>();
        services.AddScoped<UpdateClienteUseCase>();
        services.AddScoped<DeleteClienteUseCase>();

        // Veiculo Use Cases
        services.AddScoped<CreateVeiculoUseCase>();
        services.AddScoped<GetVeiculoByIdUseCase>();
        services.AddScoped<GetVeiculosPagedUseCase>();
        services.AddScoped<UpdateVeiculoUseCase>();
        services.AddScoped<DeleteVeiculoUseCase>();

        return services;
    }
}
