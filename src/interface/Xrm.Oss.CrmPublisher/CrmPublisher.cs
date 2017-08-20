using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Xrm.Sdk;
using Xrm.Oss.Interfacing.Domain;
using Xrm.Oss.Interfacing.Domain.Implementations;

namespace Xrm.Oss.CrmPublisher
{
    public class CrmPublisher : MarshalByRefObject, ICrmPublisher
    {
        public void ProcessMessage(IMessage message, IOrganizationService service, IBusControl busControl)
        {
            var record = service.Retrieve(message.Scenario.Entity, message.RecordId.Value, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

            busControl.Publish(new CrmMessage
            {
                Scenario = message.Scenario,
                RecordId = message.RecordId,
                Attributes = record.Attributes.Where(pair => pair.Value != null).ToDictionary(pair => pair.Key, pair => pair.Value)
            });
        }

        public List<IScenario> RetrieveSupportedScenarios()
        {
            return new List<IScenario>
            {
                new Scenario("update", "account"),
                new Scenario("create", "account"),
                new Scenario("update", "contact"),
                new Scenario("create", "contact")
            };
        }
    }
}
