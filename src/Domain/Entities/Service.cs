using Domain.Common.Entities;
using Domain.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Service : SoftDeleteBaseEntity
    {
        public string ServiceName { get; private set; }

        //public Service() { }

        public Service(string serviceName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);

            this.ServiceName = serviceName;
        }

        public Service(Guid idService, string serviceName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);

            this.ServiceName = serviceName;
        }

        public void Update(string serviceName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);

            ServiceName = serviceName.Trim();
        }
    }
}
