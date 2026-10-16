using Application.UseCases.Servicos.Dto;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.Servicos
{
    public class CreateServicoUserCase(IServicoRepository servicoRepository)
    {
        private readonly IServicoRepository _servicoRepository = servicoRepository;

        public async Task<ServicoOutput> ExecuteAsync(CreateServicoInput servicoInputs, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(servicoInputs.nome))
            {
                throw new ArgumentException("O nome do serviço é obrigatório.");
            }

            if (await _servicoRepository.GetByServiceNameAsync(servicoInputs.nome.Trim(), cancellationToken) != null)
            {
                throw new InvalidOperationException($"Já existe um serviço com esse nome '{servicoInputs.nome}'.");
            }

            var servico = new Domain.Entities.Servico(servicoInputs.nome.Trim());
            await _servicoRepository.AddSync(servico, cancellationToken);
            
            return ServicoOutput.FromEntity(servico);
        }
    }
}
