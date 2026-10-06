using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Database.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Database.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext dbContext, IPasswordHasher passwordHasher, ILogger logger)
    {
        try
        {
            var defaultUsers = new (string Email, string Password, string Role)[]
            {
                ("admin@siaes.com", "SenhaAdmin1!", Roles.Admin),
                ("atendente@siaes.com", "SenhaAten1!", Roles.Atendente)
            };

            var usersAdded = 0;

            foreach (var (email, password, role) in defaultUsers)
            {
                var normalizedEmail = email.Trim().ToLowerInvariant();
                var exists = await dbContext.Users.AnyAsync(u => u.Email == normalizedEmail);

                if (!exists)
                {
                    var passwordHash = passwordHasher.Hash(password);
                    var user = new User(normalizedEmail, passwordHash, role);
                    await dbContext.Users.AddAsync(user);
                    usersAdded++;
                }
            }

            if (usersAdded > 0)
            {
                await dbContext.SaveChangesAsync();
                logger.LogInformation("DatabaseSeeder: Seeded {Count} initial user(s) successfully.", usersAdded);
            }
            else
            {
                logger.LogInformation("DatabaseSeeder: Default users already exist in database.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DatabaseSeeder: An error occurred while seeding default users.");
            throw;
        }
    }
}
