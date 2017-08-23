using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Xrm.Sdk;
using NLog;
using Xrm.Oss.Interfacing.Domain.Interfaces;

namespace Xrm.Oss.CrmPublisher
{
    public class CrmPublisher : IConsumer<ICrmEvent>
    {
        private IBusControl _busControl;
        private IOrganizationService _service;
        private IPublisherControl _publisherControl;
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public CrmPublisher(IBusControl busControl, IOrganizationService service, IPublisherControl publisherControl)
        {
            _busControl = busControl;
            _service = service;
            _publisherControl = publisherControl;
        }

        public Task Consume(ConsumeContext<ICrmEvent> context)
        {
            var message = context.Message;

            _publisherControl.RouteMessage(message, _service, _busControl);

            _logger.Info($"Message with correlation ID {message.CorrelationId} was successfully published");

            return Task.FromResult(0);
        }
    }
}
