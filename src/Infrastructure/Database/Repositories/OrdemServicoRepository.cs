using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;
using Infrastructure.Database.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class OrdemServicoRepository(ApplicationDbContext context) : IOrdemServicoRepository
{

    public async Task<OrdemServico?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => await context.OrdensServico
            .Include(os => os.Historico)   
            .FirstOrDefaultAsync(os => os.Id == id, ct);

    public async Task<IEnumerable<OrdemServico>> ObterPorDocumentoClienteAsync(
        Documento documento, CancellationToken ct = default)
        => await context.OrdensServico
            .Include(os => os.Cliente)
            .Include(os => os.Historico)
            .Where(os => os.Cliente!.Documento.Valor == documento.Valor)
            .OrderByDescending(os => os.Id)
            .ToListAsync(ct);

    public async Task<IEnumerable<OrdemServico>> ListarTodasAsync(CancellationToken ct = default)
        => await context.OrdensServico
            .Include(os => os.Historico)
            .OrderByDescending(os => os.Id)
            .ToListAsync(ct);


    public async Task AdicionarAsync(OrdemServico ordemServico, CancellationToken ct = default)
        => await context.OrdensServico.AddAsync(ordemServico, ct);

    public Task SalvarAlteracoesAsync(CancellationToken ct = default)
        => context.SaveChangesAsync(ct);
}