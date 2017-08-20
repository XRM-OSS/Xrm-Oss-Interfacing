using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Xrm.Sdk;
using Xrm.Oss.Interfacing.Domain;

namespace Xrm.Oss.CrmListener
{
    public interface IPublisherControlWrapper
    {
        void RouteMessage(ICrmMessage message, IOrganizationService service, IBusControl busControl);
    }
}
