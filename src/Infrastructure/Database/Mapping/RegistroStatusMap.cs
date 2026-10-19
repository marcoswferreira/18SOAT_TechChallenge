using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Mapping;

public class RegistroStatusMap : IEntityTypeConfiguration<RegistroStatus>
{
    public void Configure(EntityTypeBuilder<RegistroStatus> builder)
    {
        builder.ToTable("OrdensServico_Historico");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.Property(r => r.OrdemServicoId).IsRequired();

        // De é nulo no registro de criação da OS
        builder.Property(r => r.De)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(r => r.Para)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(r => r.Em).IsRequired();

        // Índice composto: acelera a consulta de métrica
        // (busca por OrdemServicoId filtrando por Para = EmExecucao/Finalizada)
        builder.HasIndex(r => new { r.OrdemServicoId, r.Para });
    }
}