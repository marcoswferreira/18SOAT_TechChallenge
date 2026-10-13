using Application.UseCases.Services.Dto;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.Services
{
    public class CreateServicesUseCase(IServiceRepository serviceRepository)
    {
        private readonly IServiceRepository _serviceRepository = serviceRepository;

        public async Task<ServiceOutput> ExecuteAsync(CreateServiceInput serviceInputs, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(serviceInputs.serviceName))
            {
                throw new ArgumentException("O nome do serviço é obrigatório.");
            }

            if (await _serviceRepository.GetByServiceNameAsync(serviceInputs.serviceName, cancellationToken) != null)
            {
                throw new InvalidOperationException($"Já existe um serviço com esse nome '{serviceInputs.serviceName}'.");
            }

            var service = new Domain.Entities.Service(serviceInputs.serviceName);
            await _serviceRepository.AddSync(service, cancellationToken);
            
            return ServiceOutput.FromEntity(service);
        }
    }
}
