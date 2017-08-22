using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Xrm.Sdk;

namespace Xrm.Oss.Interfacing.Domain.Implementations
{
    public abstract class CrmConsumer<T> : IConsumer<T> where T : class
    {
        protected IOrganizationService _service;

        public CrmConsumer(IOrganizationService service)
        {
            _service = service;
        }

        public abstract Task Consume(ConsumeContext<T> context);
    }
}
