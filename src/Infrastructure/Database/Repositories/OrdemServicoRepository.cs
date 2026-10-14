using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class OrdemServicoRepository(DbContext context) : IOrdemServicoRepository
{
    // 1. Consulta pública por ID da OS
    public async Task<OrdemServico?> ObterPorIdAsync(Guid id, CancellationToken ct)
    {
        return await context.Set<OrdemServico>()
            .FirstOrDefaultAsync(os => os.Id == id, ct);

        //return await context.Set<OrdemServico>()
        //    .Include(os => os.Cliente)
        //    .FirstOrDefaultAsync(os => os.Id == id, ct);
    }

    // 2. Consulta pública por CPF/CNPJ do Cliente
    public async Task<IEnumerable<OrdemServico>> ObterPorDocumentoClienteAsync(CpfCnpj documento, CancellationToken ct)
    {
        return await context.Set<OrdemServico>()
            .OrderByDescending(os => os.Id) 
            .ToListAsync(ct);

        //return await context.Set<OrdemServico>()
        //    .Include(os => os.Cliente)
        //    .Where(os => os.Cliente.Documento.Numero == documento.Numero)
        //    .OrderByDescending(os => os.Id) // traz as mais recentes primeiro
        //    .ToListAsync(ct);
    }

    public async Task SalvarAsync(OrdemServico ordemServico, CancellationToken ct)
    {
        var existe = await context.Set<OrdemServico>().AnyAsync(os => os.Id == ordemServico.Id, ct);
        if (!existe)
            await context.Set<OrdemServico>().AddAsync(ordemServico, ct);

        await context.SaveChangesAsync(ct);
    }
}
