using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases.Services.Dto
{
    public class ServiceOutput(Guid id, string serviceName, DateTime CreateadAt, DateTime? LastModifiedAt)
    {
        public static ServiceOutput FromEntity(Domain.Entities.Service services)
        {
            return new ServiceOutput(
                services.Id,
                services.ServiceName,
                services.CreatedAt,
                services.LastModifiedAt
            );
        }

    }
}
