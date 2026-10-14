using Application.Dto;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;

namespace Application.Services;

public class OrdemServicoService(IOrdemServicoRepository repository)
{
    public async Task<ConsultaStatusClienteResponse> ConsultarStatusPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var os = await repository.ObterPorIdAsync(id, cancellationToken)
               ?? throw new KeyNotFoundException($"Ordem de Serviço com ID {id} não encontrada.");

        return ConsultaStatusClienteResponse.FromDomain(os);
    }

    public async Task<IEnumerable<ConsultaStatusClienteResponse>> ConsultarStatusPorDocumentoAsync(string documentoTexto, CancellationToken cancellationToken)
    {
        // O Value Object CpfCnpj aplica a regra de validação do documento
        var cpfCnpj = new CpfCnpj(documentoTexto);

        var ordens = await repository.ObterPorDocumentoClienteAsync(cpfCnpj, cancellationToken);

        return ordens.Select(ConsultaStatusClienteResponse.FromDomain);
    }
}
