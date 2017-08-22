using System.ComponentModel.Composition;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Xrm.Sdk;
using Xrm.Oss.Interfacing.Domain.Implementations;
using Xrm.Oss.Interfacing.Domain.Interfaces;

namespace Xrm.Oss.DemoConsumer
{
    [Export(typeof(CrmConsumer<IDemoThirdPartyContactCreated>))]
    public class DemoThirdPartyContactCreatedConsumer : CrmConsumer<IDemoThirdPartyContactCreated>
    {
        [ImportingConstructor]
        public DemoThirdPartyContactCreatedConsumer([Import(typeof(IOrganizationService))] IOrganizationService service) : base(service) { }

        public override Task Consume(ConsumeContext<IDemoThirdPartyContactCreated> context)
        {
            var message = context.Message;
            var attributes = new AttributeCollection();

            var contact = new Entity
            {
                LogicalName = "contact",
                Attributes =
                {
                    { "lastname", message.LastName },
                    { "fistname", message.FirstName },
                    { "telephone1", message.Telephone1 },
                    { "emailaddress1", message.EMailAddress1 }
                }
            };

            var guid = _service.Create(contact);

            return Task.FromResult(0);
        }
    }
}
