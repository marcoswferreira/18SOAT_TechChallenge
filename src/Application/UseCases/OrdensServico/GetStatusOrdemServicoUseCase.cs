using Application.UseCases.OrdensServico.Dto;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;

namespace Application.UseCases.OrdensServico;

public class GetStatusOrdemServicoUseCase(IOrdemServicoRepository repository)
{
    private readonly IOrdemServicoRepository _repository = repository;

    public async Task<GetStatusClienteResponse> ExecuteAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var os = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Ordem de Serviço {id} não encontrada.");

        return GetStatusClienteResponse.FromDomain(os);
    }

    public async Task<IEnumerable<GetStatusClienteResponse>> ExecuteAsync(
        string documentoTexto, CancellationToken cancellationToken = default)
    {
        var documento = new Documento(documentoTexto);
        var ordens = await _repository.GetByDocumentoClienteAsync(documento, cancellationToken);
        return ordens.Select(GetStatusClienteResponse.FromDomain);
    }
}
