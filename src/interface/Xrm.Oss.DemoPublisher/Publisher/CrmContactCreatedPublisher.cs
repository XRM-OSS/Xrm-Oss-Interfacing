using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using MassTransit;
using Microsoft.Xrm.Sdk;
using NLog;
using Xrm.Oss.DemoPublisher.Messages;
using Xrm.Oss.Interfacing.Domain.Implementations;
using Xrm.Oss.Interfacing.Domain.Interfaces;

namespace Xrm.Oss.DemoPublisher.Publisher
{
    [Export(typeof(ICrmPublisher))]
    public class CrmContactCreatedPublisher : ICrmPublisher
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly List<IScenario> _supportedScenarios = new List<IScenario>
        {
            new Scenario("create", "contact")
        };

        public void ProcessMessage(ICrmEvent message, IOrganizationService service, IBusControl busControl)
        {
            _logger.Trace($"Retrieving contact with id {message.RecordId} from CRM");

            var record = service.Retrieve(message.Scenario.Entity, message.RecordId.Value, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

            _logger.Trace($"Publishing message to bus");

            busControl.Publish(new DemoCrmContactCreated
            {
                ContactId = record.GetAttributeValue<Guid>("contactid"),
                CorrelationId = message.CorrelationId,
                EMailAddress1 = record.GetAttributeValue<string>("emailaddress1"),
                FirstName = record.GetAttributeValue<string>("firstname"),
                LastName = record.GetAttributeValue<string>("lastname"),
                Telephone1 = record.GetAttributeValue<string>("telephone1"),
                TimeStamp = message.TimeStamp
            });

            _logger.Info($"Successfully published DemoCrmContactCreated message {message.CorrelationId} to bus");
        }

        public List<IScenario> RetrieveSupportedScenarios()
        {
            return _supportedScenarios;
        }
    }
}
