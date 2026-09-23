using Domain.Common.Entities;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.DbContexts;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
    IUserContext userContext) : DbContext(options)
{
    private readonly IUserContext _userContext = userContext;

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ApplySoftDeleteQueryFilters(modelBuilder);
        // Mapeia automaticamente todas as classes de configuração do assembly atual
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditInformation()
    {
        var currentUserId = _userContext.UserId ?? "System";

        foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreatedInfo(currentUserId);
                    break;

                case EntityState.Modified:
                    entry.Entity.SetUpdatedInfo(currentUserId);
                    break;

                case EntityState.Deleted when entry.Entity is SoftDeleteBaseEntity softDeleteEntity:
                    // Trata Soft Delete: altera o estado de Deleted para Modified
                    entry.State = EntityState.Modified;
                    softDeleteEntity.Delete(currentUserId);
                    break;
            }
        }
    }

    private static void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(SoftDeleteBaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(ConvertFilterToLambda(entityType.ClrType));
            }
        }
    }

    private static System.Linq.Expressions.LambdaExpression ConvertFilterToLambda(Type entityType)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "e");
        var property = System.Linq.Expressions.Expression.Property(parameter, nameof(SoftDeleteBaseEntity.IsDeleted));
        var comparison = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));

        return System.Linq.Expressions.Expression.Lambda(comparison, parameter);
    }
}