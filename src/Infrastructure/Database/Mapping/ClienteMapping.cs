using Domain.Entities;
using Domain.ValueObjects;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Mapping;

public class ClienteMapping : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.Documento)
            .HasConversion(
                d => DeterministicEncryption.Encrypt(d.Valor),
                v => new Documento(DeterministicEncryption.Decrypt(v)))
            .HasColumnName("Documento")
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(c => c.Documento)
            .IsUnique();

        builder.Property(c => c.Email)
            .HasMaxLength(150)
            .IsRequired(false);

        builder.Property(c => c.Telefone)
            .HasMaxLength(30)
            .IsRequired(false);

        builder.ComplexProperty(c => c.Endereco, end =>
        {
            end.Property(e => e.Logradouro)
                .HasColumnName("Endereco_Logradouro")
                .HasMaxLength(150)
                .IsRequired();

            end.Property(e => e.Numero)
                .HasColumnName("Endereco_Numero")
                .HasMaxLength(20)
                .IsRequired();

            end.Property(e => e.Complemento)
                .HasColumnName("Endereco_Complemento")
                .HasMaxLength(100)
                .IsRequired(false);

            end.Property(e => e.Bairro)
                .HasColumnName("Endereco_Bairro")
                .HasMaxLength(100)
                .IsRequired();

            end.Property(e => e.Cidade)
                .HasColumnName("Endereco_Cidade")
                .HasMaxLength(100)
                .IsRequired();

            end.Property(e => e.Estado)
                .HasColumnName("Endereco_Estado")
                .HasMaxLength(2)
                .IsRequired();

            end.Property(e => e.Cep)
                .HasColumnName("Endereco_CEP")
                .HasMaxLength(8)
                .IsRequired();
        });

        builder.HasMany(c => c.Veiculos)
            .WithOne(v => v.Cliente)
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Auditoria e Soft Delete
        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.CreatedBy)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(c => c.LastModifiedAt)
            .IsRequired(false);

        builder.Property(c => c.LastModifiedBy)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(c => c.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(c => c.DeletedAt)
            .IsRequired(false);

        builder.Property(c => c.DeletedBy)
            .HasMaxLength(100)
            .IsRequired(false);
    }
}