using Application.UseCases.Services.Dto;
using Domain.Common.Paginate;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.Services
{
    public class GetServicePagedUseCase(IServiceRepository serviceRepository)
    {
        private readonly IServiceRepository _serviceRepository = serviceRepository;
        public async Task<IPaginate<Dto.ServiceOutput>> ExecuteAsync(int index, int size, CancellationToken cancellationToken)
        {
            var servicesPaged = await _serviceRepository.GetPagedAsync(index, size, cancellationToken);

            return new Paginate<Domain.Entities.Service, ServiceOutput>
                (servicesPaged,
                 services => services.Select(Dto.ServiceOutput.FromEntity));
        }
    }
}
