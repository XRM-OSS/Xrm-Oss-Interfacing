using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Xrm.Sdk;
using Nancy;
using NLog;

namespace Xrm.Oss.CrmListener.Modules
{
    public class ActionModule : NancyModule
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public ActionModule(IOrganizationService service, IBusControl busControl)
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

                Entity record = service.Retrieve(entity, guid, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

                busControl.Publish(new CrmMessage
                {
                    Action = action,
                    Entity = entity,
                    Id = guid,
                    Attributes = record.Attributes.Where(pair => pair.Value != null).ToDictionary(pair => pair.Key, pair => pair.Value)
                });

                return HttpStatusCode.OK;
            };
        }
    }
}
