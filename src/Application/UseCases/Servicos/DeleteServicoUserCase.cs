using Domain.Interfaces.Repositories;

namespace Application.UseCases.Servicos
{
    //falta implementar se o codigo do serviço ja está atrelado a alguma OS. Uma vez ja atrealado nao podemos excluir o serviço.
    public class DeleteServicoUserCase(IServicoRepository servicoRepository)
    {
        private readonly IServicoRepository _servicoRepository = servicoRepository;
        public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var servico = await _servicoRepository.GetByIdAsync(id, cancellationToken);
            if (servico == null)
            {
                throw new ArgumentException($"Serviço com ID '{id}' não encontrado.");
            }
            
            await _servicoRepository.DeleteSync(servico, cancellationToken);
        }
    }
}
