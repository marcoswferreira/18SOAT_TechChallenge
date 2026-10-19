using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Mapping;

public class ServicoMapping : IEntityTypeConfiguration<Servico>
{
    public void Configure(EntityTypeBuilder<Servico> builder)
    {
        builder.ToTable("Servicos");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(s => s.Nome);

        // Auditoria e Soft Delete
        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.CreatedBy)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(s => s.LastModifiedAt)
            .IsRequired(false);

        builder.Property(s => s.LastModifiedBy)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(s => s.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(s => s.DeletedAt)
            .IsRequired(false);

        builder.Property(s => s.DeletedBy)
            .HasMaxLength(100)
            .IsRequired(false);
    }
}

