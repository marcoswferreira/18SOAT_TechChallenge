using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Mapping;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.RefreshTokenHash)
            .HasMaxLength(128)
            .IsRequired(false);

        builder.Property(u => u.RefreshTokenExpiresAt)
            .IsRequired(false);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasIndex(u => u.RefreshTokenHash)
            .HasFilter("[RefreshTokenHash] IS NOT NULL"); 

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.CreatedBy)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(u => u.LastModifiedAt)
            .IsRequired(false);

        builder.Property(u => u.LastModifiedBy)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(u => u.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(u => u.DeletedAt)
            .IsRequired(false);

        builder.Property(u => u.DeletedBy)
            .HasMaxLength(100)
            .IsRequired(false);
    }
}