using Application.UseCases.Auth;
using Application.UseCases.Users;
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

        return services;
    }
}
