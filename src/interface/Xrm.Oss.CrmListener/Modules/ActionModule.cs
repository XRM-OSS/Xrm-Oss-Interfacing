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

        public ActionModule(IBusControl busControl)
        {
            Get["trigger/{action}/{entity}/{id}", true] = async (parameters, ct) =>
            {
                var action = parameters.action?.Value;
                var entity = parameters.entity?.Value;
                var id = parameters.id?.Value;

                _logger.Trace($"Received message for event {action} on entity {entity}, record id: {id}");

                var guid = Guid.Empty;

                if (!Guid.TryParse(id, out guid))
                {
                    _logger.Trace("Failed to parse id, returning status code 400");
                    return HttpStatusCode.BadRequest;
                }

                var message = new CrmEvent
                {
                    Scenario = new Scenario(action, entity),
                    TimeStamp = DateTime.UtcNow,
                    RecordId = guid,
                    CorrelationId = Guid.NewGuid().ToString()
                };

                _logger.Trace($"Publishing CrmEvent with correlation ID {message.CorrelationId} to bus");

                await busControl.Publish(message).ConfigureAwait(false);

                _logger.Trace($"Successfully published message {message.CorrelationId}, returning statuscode 200");

                return HttpStatusCode.OK;
            };
        }
    }
}
