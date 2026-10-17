using Application.Dto;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;

namespace Application.Services;

public class OrdemServicoService(IOrdemServicoRepository repository)
{
    public async Task<ConsultaStatusClienteResponse> ConsultarStatusPorIdAsync(
        Guid id, CancellationToken ct)
    {
        var os = await repository.ObterPorIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Ordem de Serviço {id} não encontrada.");

        return ConsultaStatusClienteResponse.FromDomain(os);
    }

    public async Task<IEnumerable<ConsultaStatusClienteResponse>> ConsultarStatusPorDocumentoAsync(
        string documentoTexto, CancellationToken ct)
    {
        var documento = new Documento(documentoTexto); // valida CPF/CNPJ aqui
        var ordens = await repository.ObterPorDocumentoClienteAsync(documento, ct);
        return ordens.Select(ConsultaStatusClienteResponse.FromDomain);
    }
}