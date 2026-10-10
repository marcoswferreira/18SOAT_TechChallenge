using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Mapping;

public class ClienteMapping : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(u => u.CPFCNPJ)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(u => u.CPFCNPJ)
            .IsUnique();

        builder.Property(u => u.Email)
            .HasMaxLength(50);

        builder.Property(u => u.Telefone)
            .IsRequired()
            .HasMaxLength(30);
    }
}