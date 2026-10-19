using Application.UseCases.OrdensServico.Dto;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.OrdensServico;

public class IniciarDiagnosticoUseCase(IOrdemServicoRepository repository)
{
    private readonly IOrdemServicoRepository _repository = repository;

    public async Task<OrdemServicoOutput> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var os = await _repository.GetByIdWithDetailsAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Ordem de Serviço com ID '{id}' não foi encontrada.");

        os.IniciarDiagnostico();

        _repository.Update(os);
        await _repository.SaveChangesAsync(cancellationToken);

        return OrdemServicoOutput.FromEntity(os);
    }
}

