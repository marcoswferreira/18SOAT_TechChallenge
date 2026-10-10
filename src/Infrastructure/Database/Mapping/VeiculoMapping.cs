using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Mapping;

public class VeiculoMapping : IEntityTypeConfiguration<Veiculo>
{
    public void Configure(EntityTypeBuilder<Veiculo> builder)
    {
        builder.ToTable("Veiculos");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.ClienteId)
            .IsRequired();

        builder.Property(u => u.Modelo)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Marca)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Placa)
            .IsRequired()
            .HasMaxLength(7);

        builder.HasIndex(u => u.Placa)
            .IsUnique();

        builder.HasOne(u => u.Cliente)
            .WithMany(c => c.Veiculos)
            .HasForeignKey(u => u.ClienteId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
/*
        builder.HasOne<Cliente>()
            .WithMany()
            .HasForeignKey(u => u.ClienteId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);*/
    }
}