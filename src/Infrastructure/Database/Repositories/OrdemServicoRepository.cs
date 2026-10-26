using Domain.Common.Paginate;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;
using Infrastructure.Database.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class OrdemServicoRepository(ApplicationDbContext context) : BaseRepository<OrdemServico>(context), IOrdemServicoRepository
{
    public async Task<OrdemServico?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(os => os.Historico)
            .FirstOrDefaultAsync(os => os.Id == id, cancellationToken);
    }

    public async Task<OrdemServico?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsSplitQuery()
            .Include(os => os.Cliente)
            .Include(os => os.Veiculo)
            .Include(os => os.Historico)
            .Include(os => os.Servicos)
            .Include(os => os.Pecas)
            .FirstOrDefaultAsync(os => os.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<OrdemServico>> GetByDocumentoClienteAsync(
        Documento documento, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(os => os.Cliente)
            .Include(os => os.Veiculo)
            .Include(os => os.Historico)
            .Where(os => os.Cliente!.Documento == documento)
            .OrderByDescending(os => os.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IPaginate<OrdemServico>> GetPagedAsync(int index, int size, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(os => os.Cliente)
            .Include(os => os.Veiculo)
            .Include(os => os.Historico)
            .OrderByDescending(os => os.Id)
            .ToPaginateAsync(index, size, 0, cancellationToken);
    }

    public async Task<IEnumerable<TimeSpan>> ObterTemposDeExecucaoAsync(
    DateTime? de, DateTime? ate, CancellationToken ct = default)
    {
        var query = context.OrdensServico
            .Include(os => os.Historico)
            .Where(os => os.Historico.Any(r =>
                r.Para == StatusOrdemServico.Finalizada &&
                (de == null || r.Em >= de) &&
                (ate == null || r.Em <= ate)));

        var ordens = await query.ToListAsync(ct);

        return ordens
            .Select(os => os.TempoDeExecucao())
            .Where(t => t.HasValue)
            .Select(t => t!.Value);
    }

}