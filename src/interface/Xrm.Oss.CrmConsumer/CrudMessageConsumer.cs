using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Xrm.Sdk;
using NLog;
using Xrm.Oss.Interfacing.Domain;

namespace Xrm.Oss.CrmConsumer
{
    public class CrudMessageConsumer : IConsumer<ICrudMessage>
    {
        private IOrganizationService _service;
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public CrudMessageConsumer(IOrganizationService service)
        {
            _service = service;
        }

        private void HandleCreate(ICrudMessage message)
        {
            var attributes = new AttributeCollection();

            if (message.Attributes != null) {
                foreach (var attribute in message.Attributes)
                {
                    attributes.Add(attribute);
                }
            }

            var entity = new Entity
            {
                LogicalName = message.Entity,
                Attributes = attributes
            };

            var guid = _service.Create(entity);

            _logger.Trace($"Created {message.Entity} with ID {guid}");
        }

        public Task Consume(ConsumeContext<ICrudMessage> context)
        {
            var message = context.Message;

            switch (message.Event?.ToLowerInvariant())
            {
                case "create":
                    HandleCreate(message);
                    break;
                default:
                    _logger.Error($"Don't know how to handle event '{message.Event}', skipping.");
                    break;
            }

            return Task.FromResult(0);
        }
    }
}
