using System.Threading.Tasks;
using MassTransit;
using Microsoft.Xrm.Sdk;
using NLog;
using Xrm.Oss.Interfacing.DemoContracts;

namespace Xrm.Oss.Interfacing.CrmConsumer
{
    public class DemoThirdPartyContactCreatedConsumer : IConsumer<IDemoThirdPartyContactCreated>
    {
        private IOrganizationService _service;
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public DemoThirdPartyContactCreatedConsumer(IOrganizationService service)
        {
            _service = service;
        }

        public Task Consume(ConsumeContext<IDemoThirdPartyContactCreated> context)
        {
            var message = context.Message;
            var attributes = new AttributeCollection();

            _logger.Info($"Processing message {message.CorrelationId}");

            var contact = new Entity
            {
                LogicalName = "contact",
                Attributes =
                {
                    { "lastname", message.LastName },
                    { "firstname", message.FirstName },
                    { "telephone1", message.Telephone1 },
                    { "emailaddress1", message.EMailAddress1 }
                }
            };

            _logger.Trace($"Creating contact in CRM");
            var guid = _service.Create(contact);
            _logger.Info($"Successfully processed message {message.CorrelationId}");

            return Task.FromResult(0);
        }
    }
}
