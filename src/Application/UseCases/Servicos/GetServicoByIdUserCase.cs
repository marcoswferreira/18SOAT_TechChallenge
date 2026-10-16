using Application.UseCases.Servicos.Dto;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.Servicos
{
    public class GetServicoByIdUserCase(IServicoRepository servicoRepository)
    {
        private readonly IServicoRepository _servicoRepository = servicoRepository; 

        public async Task<ServicoOutput> ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var servico = await _servicoRepository.GetByIdAsync(id, cancellationToken);
            if (servico == null)
            {
                throw new ArgumentException($"Serviço com ID '{id}' não encontrado.");
            }
            return ServicoOutput.FromEntity(servico);
        }
    }
}
