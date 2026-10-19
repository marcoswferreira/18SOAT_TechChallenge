using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Mapping;

public class OrdemServicoMap : IEntityTypeConfiguration<OrdemServico>
{
    public void Configure(EntityTypeBuilder<OrdemServico> builder)
    {
        builder.ToTable("OrdensServico");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever();

        builder.Property(o => o.ClienteId).IsRequired();
        builder.Property(o => o.VeiculoId).IsRequired();

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(o => o.Status);

        builder.Property(o => o.ValorTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        // ItemServico
        builder.OwnsMany(o => o.Servicos, b =>
        {
            b.ToTable("OrdensServico_Servicos");
            b.WithOwner().HasForeignKey("OrdemServicoId");

            b.Property<int>("Id").ValueGeneratedOnAdd();
            b.HasKey("Id");

            b.Property(s => s.ServicoId)
                .IsRequired(false);

            b.Property(s => s.Descricao)
                .HasMaxLength(200)
                .IsRequired();

            b.Property(s => s.ValorUnitario)
                .HasPrecision(18, 2)
                .IsRequired();

            b.Property(s => s.Quantidade).IsRequired();

            b.Ignore(s => s.ValorTotal);
        });

        // ItemPeca
        builder.OwnsMany(o => o.Pecas, b =>
        {
            b.ToTable("OrdensServico_Pecas");
            b.WithOwner().HasForeignKey("OrdemServicoId");

            b.Property<int>("Id").ValueGeneratedOnAdd();
            b.HasKey("Id");

            b.Property(p => p.ItemEstoqueId)
                .IsRequired();

            b.Property(p => p.NomePeca)
                .HasMaxLength(200)
                .IsRequired();

            b.Property(p => p.ValorUnitario)
                .HasPrecision(18, 2)
                .IsRequired();

            b.Property(p => p.Quantidade)
                .HasPrecision(12, 3)
                .IsRequired();

            b.Ignore(p => p.ValorTotal);
        });

        // RegistroStatus
        builder.HasMany(o => o.Historico)
            .WithOne()
            .HasForeignKey(r => r.OrdemServicoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(o => o.Historico)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}