using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class OrdemServicoRepository(DbContext context) : IOrdemServicoRepository
{
    public Task AdicionarAsync(OrdemServico ordemServico, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<OrdemServico>> ListarTodasAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<OrdemServico>> ObterPorDocumentoClienteAsync(Documento documento, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<OrdemServico?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task SalvarAlteracoesAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
