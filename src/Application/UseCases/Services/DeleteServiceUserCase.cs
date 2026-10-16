using Domain.Interfaces.Repositories;

namespace Application.UseCases.Services
{
    //falta implementar se o codigo do serviço ja está atrelado a alguma OS. Uma vez ja atrealado nao podemos excluir o serviço.
    public class DeleteServiceUserCase(IServiceRepository serviceRepository)
    {
        private readonly IServiceRepository _serviceRepository = serviceRepository;
        public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var service = await _serviceRepository.GetByIdAsync(id, cancellationToken);
            if (service == null)
            {
                throw new ArgumentException($"Serviço com ID '{id}' não encontrado.");
            }
            
            await _serviceRepository.DeleteSync(service, cancellationToken);
        }
    }
}
