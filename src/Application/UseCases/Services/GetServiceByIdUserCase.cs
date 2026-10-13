using Domain.Interfaces.Repositories;

namespace Application.UseCases.Services
{
    public class GetServiceByIdUserCase(IServiceRepository serviceRepository)
    {
        private readonly IServiceRepository _serviceRepository = serviceRepository; 

        public async Task<Dto.ServiceOutput> ExecuteAsync(Guid idService, CancellationToken cancellationToken)
        {
            var service = await _serviceRepository.GetByIdAsync(idService, cancellationToken);
            if (service == null)
            {
                throw new ArgumentException($"Serviço com ID '{idService}' não encontrado.");
            }
            return Dto.ServiceOutput.FromEntity(service);
        }
    }
}
