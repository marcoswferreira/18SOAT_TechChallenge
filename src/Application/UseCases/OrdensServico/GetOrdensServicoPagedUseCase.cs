using Application.UseCases.OrdensServico.Dto;
using Domain.Common.Paginate;
using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.OrdensServico;

public class GetOrdensServicoPagedUseCase(IOrdemServicoRepository repository)
{
    private readonly IOrdemServicoRepository _repository = repository;

    public async Task<IPaginate<OrdemServicoOutput>> ExecuteAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        var pagedOs = await _repository.GetPagedAsync(pageIndex, pageSize, cancellationToken);

        return new Paginate<OrdemServico, OrdemServicoOutput>(
            pagedOs,
            items => items.Select(OrdemServicoOutput.FromEntity));
    }
}

