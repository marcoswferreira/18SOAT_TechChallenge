using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Mapping;

public class UserRoleMapping : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        builder.HasKey(ur => new { ur.UserId, ur.Role });

        builder.Property(ur => ur.Role)
            .IsRequired()
            .HasMaxLength(50);

        // Espelha o query filter de soft delete do User para evitar o aviso EF10622
        builder.HasQueryFilter(ur => !ur.User.IsDeleted);
    }
}
