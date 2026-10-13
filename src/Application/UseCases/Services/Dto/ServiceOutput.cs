using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases.Services.Dto
{
    public class ServiceOutput(Guid idService, string serviceName, DateTime CreateadAt, DateTime? LastModifiedAt)
    {
        public static ServiceOutput FromEntity(Domain.Entities.Service services)
        {
            return new ServiceOutput(
                services.IdService,
                services.ServiceName,
                services.CreatedAt,
                services.LastModifiedAt
            );
        }

    }
}
