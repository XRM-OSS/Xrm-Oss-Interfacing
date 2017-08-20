using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Xrm.Sdk;
using Nancy;
using NLog;
using Xrm.Oss.Interfacing.Domain.Implementations;

namespace Xrm.Oss.CrmListener.Modules
{
    public class ActionModule : NancyModule
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public ActionModule(IPublisherControlWrapper publisherRegistration, IOrganizationService service, IBusControl busControl)
        {
            Get["trigger/{action}/{entity}/{id}"] = parameters =>
            {
                var action = parameters.action?.Value;
                var entity = parameters.entity?.Value;
                var id = parameters.id?.Value;

                var guid = Guid.Empty;

                if(!Guid.TryParse(id, out guid)){
                    return HttpStatusCode.BadRequest;
                }

                var message = new CrmMessage
                {
                    Scenario = new Scenario(action, entity),
                    RecordId = guid
                };

                publisherRegistration.RouteMessage(message, service, busControl);

                return HttpStatusCode.OK;
            };
        }
    }
}
