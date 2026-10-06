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
            var defaultUsers = new (string Email, string Password, string[] Roles)[]
            {
                ("admin@siaes.com", "SenhaAdmin1!", [Roles.Admin]),
                ("atendente@siaes.com", "SenhaAten1!", [Roles.Atendente])
            };

            var usersAdded = 0;
            var rolesAdded = 0;

            foreach (var (email, password, roles) in defaultUsers)
            {
                var normalizedEmail = email.Trim().ToLowerInvariant();

                var existingUser = await dbContext.Users
                    .Include(u => u.Roles)
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.Email == normalizedEmail);

                if (existingUser is null)
                {
                    // Usuário não existe: cria com roles
                    var passwordHash = passwordHasher.Hash(password);
                    var user = new User(normalizedEmail, passwordHash, roles);
                    await dbContext.Users.AddAsync(user);
                    usersAdded++;
                }
                else if (existingUser.Roles.Count == 0)
                {
                    // Usuário existe mas sem roles: adiciona as roles padrão
                    foreach (var role in roles)
                        existingUser.AddRole(role);
                    rolesAdded++;
                }
            }

            if (usersAdded > 0 || rolesAdded > 0)
            {
                await dbContext.SaveChangesAsync();

                if (usersAdded > 0)
                    logger.LogInformation("DatabaseSeeder: Seeded {Count} initial user(s) successfully.", usersAdded);

                if (rolesAdded > 0)
                    logger.LogInformation("DatabaseSeeder: Seeded roles for {Count} existing user(s) without roles.", rolesAdded);
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
