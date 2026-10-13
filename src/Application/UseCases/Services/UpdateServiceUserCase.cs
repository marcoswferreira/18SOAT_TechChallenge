using Application.UseCases.Services.Dto;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.Services
{
    //falta implementar se o codigo do serviço ja está atrelado a alguma OS. Uma vez ja atrealado nao podemos mudar o nome do serviço.

    public class UpdateServiceUserCase(IServiceRepository serviceRepository)
    {
        private readonly IServiceRepository _serviceRepository = serviceRepository;
        public async Task<ServiceOutput> ExecuteAsync(Guid idService, UpdateServiceInput input, CancellationToken cancellationToken)
        {
            var service = await _serviceRepository.GetByIdAsync(idService, cancellationToken);
            if (service == null)
            {
                throw new KeyNotFoundException($"Serviço com o ID '{idService}' não encontrado.");
            }

            if (string.IsNullOrWhiteSpace(input.ServiceName))
            {
                throw new ArgumentException("O nome do serviço é obrigatório.");
            }

            service.Update(input.ServiceName);

            await _serviceRepository.UpdateSync(service, cancellationToken);

            return ServiceOutput.FromEntity(service);
        }
    }
}
