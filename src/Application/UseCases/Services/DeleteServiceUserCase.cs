using Domain.Interfaces.Repositories;

namespace Application.UseCases.Services
{
    //falta implementar se o codigo do serviço ja está atrelado a alguma OS. Uma vez ja atrealado nao podemos excluir o serviço.
    public class DeleteServiceUserCase(IServiceRepository serviceRepository)
    {
        private readonly IServiceRepository _serviceRepository = serviceRepository;
        public async Task ExecuteAsync(Guid idService, CancellationToken cancellationToken)
        {
            var service = await _serviceRepository.GetByIdAsync(idService, cancellationToken);
            if (service == null)
            {
                throw new ArgumentException($"Serviço com ID '{idService}' não encontrado.");
            }
            
            await _serviceRepository.DeleteSync(service, cancellationToken);
        }
    }
}
