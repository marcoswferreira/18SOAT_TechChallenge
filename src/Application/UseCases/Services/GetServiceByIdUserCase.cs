using Domain.Interfaces.Repositories;

namespace Application.UseCases.Services
{
    public class GetServiceByIdUserCase(IServiceRepository serviceRepository)
    {
        private readonly IServiceRepository _serviceRepository = serviceRepository; 

        public async Task<Dto.ServiceOutput> ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var service = await _serviceRepository.GetByIdAsync(id, cancellationToken);
            if (service == null)
            {
                throw new ArgumentException($"Serviço com ID '{id}' não encontrado.");
            }
            return Dto.ServiceOutput.FromEntity(service);
        }
    }
}
