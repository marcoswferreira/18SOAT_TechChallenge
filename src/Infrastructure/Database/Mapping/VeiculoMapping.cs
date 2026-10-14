using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Mapping;

public class VeiculoMapping : IEntityTypeConfiguration<Veiculo>
{
    public void Configure(EntityTypeBuilder<Veiculo> builder)
    {
        builder.ToTable("Veiculos");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.ClienteId)
            .IsRequired();

        builder.Property(v => v.Placa)
            .HasConversion(p => p.Valor, v => new Placa(v))
            .HasColumnName("Placa")
            .HasMaxLength(7)
            .IsRequired();

        builder.HasIndex(v => v.Placa)
            .IsUnique();

        builder.Property(v => v.Marca)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Modelo)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Ano)
            .IsRequired();

        builder.HasOne(v => v.Cliente)
            .WithMany(c => c.Veiculos)
            .HasForeignKey(v => v.ClienteId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // Auditoria e Soft Delete
        builder.Property(v => v.CreatedAt)
            .IsRequired();

        builder.Property(v => v.CreatedBy)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(v => v.LastModifiedAt)
            .IsRequired(false);

        builder.Property(v => v.LastModifiedBy)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(v => v.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(v => v.DeletedAt)
            .IsRequired(false);

        builder.Property(v => v.DeletedBy)
            .HasMaxLength(100)
            .IsRequired(false);
    }
}