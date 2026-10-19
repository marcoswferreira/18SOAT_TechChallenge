using Application.UseCases.Servicos.Dto;
using Domain.Common.Paginate;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.Servicos
{
    public class GetServicoPagedUseCase(IServicoRepository servicoRepository)
    {
        private readonly IServicoRepository _servicoRepository = servicoRepository;
        public async Task<IPaginate<ServicoOutput>> ExecuteAsync(int index, int size, CancellationToken cancellationToken)
        {
            var servicesPaged = await _servicoRepository.GetPagedAsync(index, size, cancellationToken);

            return new Paginate<Domain.Entities.Servico, ServicoOutput>
                (servicesPaged,
                 servicos => servicos.Select(ServicoOutput.FromEntity));
        }
    }
}
