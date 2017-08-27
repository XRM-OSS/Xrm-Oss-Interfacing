using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace Xrm.Oss.Interfacing.CrmConsumer
{
    public class CrmConnection : IWindsorInstaller
    {
        private IOrganizationService Connect()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["CRM"].ConnectionString;
            var conn = new CrmServiceClient(connectionString);

            return conn.OrganizationWebProxyClient != null 
                ? (IOrganizationService) conn.OrganizationWebProxyClient 
                : (IOrganizationService) conn.OrganizationServiceProxy;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IOrganizationService>().UsingFactoryMethod(Connect));
        }
    }
}
