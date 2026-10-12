using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Mapping;

public class ItemEstoqueMapping : IEntityTypeConfiguration<ItemEstoque>
{
    public void Configure(EntityTypeBuilder<ItemEstoque> builder)
    {
        builder.ToTable("ItensEstoque", t =>
        {
            t.HasCheckConstraint("CK_ItensEstoque_QuantidadeEmEstoque", "\"QuantidadeEmEstoque\" >= 0");
            t.HasCheckConstraint("CK_ItensEstoque_QuantidadeReservada", "\"QuantidadeReservada\" >= 0 AND \"QuantidadeReservada\" <= \"QuantidadeEmEstoque\"");
        });

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Codigo)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.Codigo)
            .IsUnique();

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Descricao)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(p => p.UnidadeMedida)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.PrecoUnitario)
            .HasPrecision(12, 2);

        builder.Property(p => p.QuantidadeEmEstoque)
            .HasPrecision(12, 3);

        builder.Property(p => p.QuantidadeReservada)
            .HasPrecision(12, 3);

        builder.Property(p => p.Versao)
            .IsRowVersion();

        builder.Ignore(p => p.QuantidadeDisponivel);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.CreatedBy)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(p => p.LastModifiedAt)
            .IsRequired(false);

        builder.Property(p => p.LastModifiedBy)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(p => p.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(p => p.DeletedAt)
            .IsRequired(false);

        builder.Property(p => p.DeletedBy)
            .HasMaxLength(100)
            .IsRequired(false);
    }
}
