using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using MassTransit;
using Microsoft.Xrm.Sdk;
using Xrm.Oss.DemoPublisher.Messages;
using Xrm.Oss.Interfacing.Domain.Implementations;
using Xrm.Oss.Interfacing.Domain.Interfaces;

namespace Xrm.Oss.DemoPublisher.Publisher
{
    [Export(typeof(ICrmPublisher))]
    public class CrmContactUpdatedPublisher : ICrmPublisher
    {
        private readonly List<IScenario> _supportedScenarios = new List<IScenario>
        {
            new Scenario("update", "contact")
        };

        public void ProcessMessage(ICrmEvent message, IOrganizationService service, IBusControl busControl)
        {
            var record = service.Retrieve(message.Scenario.Entity, message.RecordId.Value, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

            busControl.Publish(new DemoCrmContactUpdated
            {
                ContactId = record.GetAttributeValue<Guid>("contactid"),
                CorrelationId = message.CorrelationId,
                EMailAddress1 = record.GetAttributeValue<string>("emailaddress1"),
                FirstName = record.GetAttributeValue<string>("firstname"),
                LastName = record.GetAttributeValue<string>("lastname"),
                Telephone1 = record.GetAttributeValue<string>("telephone1"),
                TimeStamp = message.TimeStamp
            });
        }

        public List<IScenario> RetrieveSupportedScenarios()
        {
            return _supportedScenarios;
        }
    }
}
