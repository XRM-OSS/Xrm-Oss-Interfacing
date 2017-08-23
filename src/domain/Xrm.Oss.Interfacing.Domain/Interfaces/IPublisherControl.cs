using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Xrm.Sdk;

namespace Xrm.Oss.Interfacing.Domain.Interfaces
{
    public interface IPublisherControl
    {
        void RouteMessage(ICrmEvent crmEvent, IOrganizationService service, IBusControl busControl);
    }
}
