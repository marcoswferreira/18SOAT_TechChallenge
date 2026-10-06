using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Infrastructure.Database.DbContexts;
using Infrastructure.Database.Repositories;
using Infrastructure.Database.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Database.Extensions;

/// <summary>
/// Extension methods for registering, migrating, and seeding the database.
/// </summary>
public static class SqlServerExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
                                                                    IConfiguration configuration)
    {
        AddPostgreeSqlInfrastructure(services, configuration);

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddExceptionHandler<ExceptionHandler>();

        return services;
    }

    private static void AddPostgreeSqlInfrastructure(this IServiceCollection services,
                                                          IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found in configuration. " +
                "Add it to appsettings.json under ConnectionStrings:DefaultConnection.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
    }

    /// <summary>
    /// Applies pending EF Core migrations to PostgreSQL database schema
    /// and seeds default demo users.
    /// </summary>
    public static async Task MigrateDatabaseAsync(this IHost app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            logger.LogInformation("Applying pending EF Core database migrations...");
            await db.Database.MigrateAsync();
            logger.LogInformation("Database migration completed successfully.");

            // Seed default demo users as specified in README.md
            await DatabaseSeeder.SeedAsync(db, passwordHasher, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing or seeding the database.");
            throw;
        }
    }
}