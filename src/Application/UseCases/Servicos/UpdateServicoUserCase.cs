using Application.UseCases.Servicos.Dto;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.Servicos
{
    //falta implementar se o codigo do serviço ja está atrelado a alguma OS. Uma vez ja atrealado nao podemos mudar o nome do serviço.

    public class UpdateServicoUserCase(IServicoRepository servicoRepository)
    {
        private readonly IServicoRepository _servicoRepository = servicoRepository;
        public async Task<ServicoOutput> ExecuteAsync(Guid id, UpdateServicoInput input, CancellationToken cancellationToken)
        {
            var servico = await _servicoRepository.GetByIdAsync(id, cancellationToken);
            if (servico == null)
            {
                throw new KeyNotFoundException($"Serviço com o ID '{id}' não encontrado.");
            }

            if (string.IsNullOrWhiteSpace(input.Nome))
            {
                throw new ArgumentException("O nome do serviço é obrigatório.");
            }

            servico.Update(input.Nome.Trim());

            await _servicoRepository.UpdateSync(servico, cancellationToken);

            return ServicoOutput.FromEntity(servico);
        }
    }
}
