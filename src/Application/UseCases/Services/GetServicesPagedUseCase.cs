using Application.UseCases.Services.Dto;
using Domain.Common.Paginate;
using Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Application.UseCases.Services
{
    public class GetServicesPagedUseCase(IServiceRepository serviceRepository)
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
