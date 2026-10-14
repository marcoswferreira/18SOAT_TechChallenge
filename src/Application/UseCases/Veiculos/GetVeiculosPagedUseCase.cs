using Application.UseCases.Veiculos.Dto;
using Domain.Common.Paginate;
using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.Veiculos;

public class GetVeiculosPagedUseCase(IVeiculoRepository veiculoRepository)
{
    private readonly IVeiculoRepository _veiculoRepository = veiculoRepository;

    public async Task<IPaginate<VeiculoOutput>> ExecuteAsync(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        var pagedVeiculos = await _veiculoRepository.GetPagedAsync(pageIndex, pageSize, cancellationToken);

        return new Paginate<Veiculo, VeiculoOutput>(
            pagedVeiculos,
            veiculos => veiculos.Select(VeiculoOutput.FromEntity));
    }
}

